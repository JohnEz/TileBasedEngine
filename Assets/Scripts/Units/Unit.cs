using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public enum UnitSize {
	Small,
	Normal,
	Large,
	Giant
}

[System.Serializable]
public class Unit : MonoBehaviour {

	public int tileX;
	public int tileY;
	public TileMap map;
    public UnitManager uManager;

	public List<Node> currentPath;
	public List<Node> reachableTiles;
	public List<Node> reachableTilesWithDash;

    //Stats

    //Base - stats that the max should return to with no modifications
    public int baseHP = 100;
    public int baseMana = 100;
    public int baseMove = 4;
    public float baseArmour = 0.16f;
    public int baseInit = 4;
    public int baseAP = 1;
	public UnitSize mySize = UnitSize.Normal;

    //Max - current maximum stats after effects
	public int maxHP = 100;
	public int maxMana = 100;
	public int movespeed = 4;
	public float armourDamageReduction = 0.16f;
	public int init = 5;
	public int maxAP = 1;
	public int teir = 1;

    //current - the current value of the stats
    public int shield = 0;
	public int hp;
	public int mana;
	public int remainingMove;
	public int actionPoints;

    //modifiers
    public float damageDealtMod = 1;
    public float damageRecievedMod = 1;
    public float healingRecievedMod = 1;

	//logic bools
	public bool isDead = false;
	public bool playable = true; // is it an npc or playable character
	public bool moving 		= false; // is it currently moving
	public bool attacking 	= false; // is it currently attacking

	//combat
	public Ability[] myAbilities = new Ability[6];
    public List<Effect> myEffects = new List<Effect>();
	public List<Effect> expiredEffects = new List<Effect> ();
	public GameObject combatText;
	public int comboPoints = 0;

	void Start() {
		hp = maxHP;
		mana = maxMana;
		remainingMove = movespeed;
		actionPoints = maxAP;
	}

    public void StartTurn()
    {
        //set each stat to its base then apply effects
        maxHP = baseHP;
        maxMana = baseMana;
        movespeed = baseMove;
        armourDamageReduction = baseArmour;
        init = baseInit;
        maxAP = baseAP;
        damageDealtMod = 1;
        damageRecievedMod = 1;
        healingRecievedMod = 1;

		//apply the effects
        foreach (Effect eff in myEffects)
        {
            eff.RunEffect(this);
			if (eff.duration <= 0) {
				expiredEffects.Add(eff);
			}
        }

		//remove expired effects
		foreach (Effect eff in expiredEffects) {
			myEffects.Remove(eff);
		}
		
		expiredEffects = new List<Effect> ();

        // if maxhp is now lower than current hp
        if (hp > maxHP)
        {
            hp = maxHP;
        }

        // if maxmana is now lower than current mana
        if (mana > maxMana)
        {
            mana = maxMana;
        }

        // reset move and action
		remainingMove = movespeed;
		actionPoints = maxAP;

		for(int i=0; i < myAbilities.Length; ++i) {
			if (myAbilities[i] != null) {
				myAbilities[i].ReduceCooldown(1);
			}
		}
	}

	void Update() {
		//call the correct update method depending on unit
		if (playable) {
			PlayerUpdate();
		} else {
			AIUpdate();
		}
	}

	public void PlayerUpdate() {
		if (currentPath != null) {
			int currNode = 0;
			Vector3 start = map.TileCoordToWorldCoord(tileX, tileY) +
				new Vector3(0,0,-1.5f);
			Vector3 end = map.TileCoordToWorldCoord(currentPath[currNode].x, currentPath[currNode].y) +
				new Vector3(0,0,-1.5f);

			Debug.DrawLine(start, end, Color.blue);

			while ( currNode < currentPath.Count - 1) {
				start = map.TileCoordToWorldCoord(currentPath[currNode].x, currentPath[currNode].y) +
					new Vector3(0,0,-1.5f);
				end = map.TileCoordToWorldCoord(currentPath[currNode+1].x, currentPath[currNode+1].y) +
					new Vector3(0,0,-1.5f);
				
				Debug.DrawLine(start, end, Color.blue);
				
				++currNode;
			}
		}
		
		if (moving) {
			map.UnhighlightTiles();
			// Have we moved our visible piece close enough to the target tile that we can
			// advance to the next step in our pathfinding?
			if (Vector3.Distance (transform.position, map.TileCoordToWorldCoord (tileX, tileY)) < 0.1f) {
				SlideMovement ();
				if (Vector3.Distance (transform.position, map.TileCoordToWorldCoord (tileX, tileY)) < 0.1f) {
					// if the unit can still move
					if (remainingMove > 0 || actionPoints > 0) {
						DrawReachableTiles();
					}
					moving = false;
					transform.position = map.TileCoordToWorldCoord (tileX, tileY);
				}
			}
			
			// Smoothly animate towards the correct map tile.
			transform.position = Vector3.Lerp (transform.position, map.TileCoordToWorldCoord (tileX, tileY), 5f * Time.deltaTime);
		}

		//TODO Attacking shit
		if (attacking) {
			attacking = false;

			for (int i=0; i < 6; ++i) {
				if (myAbilities[i] != null) {
					if (!myAbilities[i].AbilityFinished) {
						attacking = true;
						myAbilities[i].UpdateAbility();
					}
				}
			}
		}
	}

	public void AIUpdate() {

		AIBehaviours myAI = GetComponent<AIBehaviours> ();

		if (moving) {
			if (Vector3.Distance (transform.position, map.TileCoordToWorldCoord (tileX, tileY)) < 0.01f) {
				SlideMovement ();
				if (Vector3.Distance (transform.position, map.TileCoordToWorldCoord (tileX, tileY)) < 0.01f) {
					moving = false;
					transform.position = map.TileCoordToWorldCoord (tileX, tileY);
				}

			}

			// Smoothly animate towards the correct map tile.
			transform.position = Vector3.Lerp (transform.position, map.TileCoordToWorldCoord (tileX, tileY), 5f * Time.deltaTime);
		} else if (attacking) {
			myAI.Attack();
			attacking = false;
		}
		else {
			remainingMove = 0;
			actionPoints = 0;
		}
	}

	public void SlideMovement() {
		// if there is no path return
		if(currentPath==null)
			return;

		// get the new first node and move
		tileX = currentPath[0].x;
		tileY = currentPath[0].y;

		// remove the old tile
		currentPath.RemoveAt (0);

		//at the target tile
		if (currentPath.Count == 0) {
			//clear path finding info
			currentPath = null;
		}


	}

	public void MoveNextTile() {
		map.UnhighlightTiles();

		while (remainingMove > 0 || actionPoints > 0) {

			if (currentPath == null || currentPath.Count == 1) {
				break;
			}

			if (remainingMove < 1 && currentPath.Count > 1) {
				remainingMove = movespeed;
				--actionPoints;
				
			}

			// remove the old tile
			currentPath.RemoveAt (0);

			remainingMove -= (int)map.CostToEnterTile(currentPath [0].x, currentPath [0].y);

			// get the new first node and move
			tileX = currentPath[0].x;
			tileY = currentPath[0].y;
			transform.position = map.TileCoordToWorldCoord(tileX, tileY);

			//at the target tile
			if (currentPath.Count == 1) {
				//clear path finding info
				currentPath = null;
			}
		}

		if (remainingMove > 0 || actionPoints > 0) {
			DrawReachableTiles();
		}
	}

	public void SlideToTile(int x, int y) {
		map.GetNode(tileX, tileY).myUnit = null;
		map.GetNode(x, y).myUnit = this;

		currentPath = new List<Node> ();
		currentPath.Add (map.GetNode (x, y));
		moving = true;

	}

	public void RemoveMovement() {
		//remove movement cost
		if (currentPath.Last ().cost > remainingMove) {
			remainingMove += movespeed - (int)currentPath.Last ().cost;
			--actionPoints;
		} else {
			remainingMove -= (int)currentPath.Last ().cost;
		}
	}

	public void DrawReachableTiles() {
		map.FindReachableTilesUnit ();
		map.HighlightTiles (reachableTiles, Color.blue, new Color(0.75f,0.75f,1), 1);
		map.HighlightTiles (reachableTilesWithDash, new Color(1,1,0), new Color(1,1,0.75f), 1);
	}

    public void TakeDamage(int dmg)
    {
        int damage = (int)((dmg * damageRecievedMod) * (1 - armourDamageReduction));
        hp -= damage;
        ShowDamage(damage);
    }

	public void TakeHealing(int heal)
	{
		int healing = (int)(heal * healingRecievedMod);
		hp += healing;
		ShowDamage(healing);
	}

	public void GainMana(int m) {
		mana += m;
		if (mana > maxMana) {
			mana = maxMana;
		}
	}

	public void ShowDamage(int dmg) {
		GameObject temp = Instantiate (combatText) as GameObject;
		RectTransform tempRect = temp.GetComponent<RectTransform> ();
		temp.GetComponent<Animator> ().SetTrigger ("Hit");
		temp.transform.SetParent (transform.FindChild("UnitCanvas"));

		tempRect.transform.localPosition = combatText.transform.localPosition;
		tempRect.transform.localScale = combatText.transform.localScale;
		tempRect.transform.rotation = combatText.transform.localRotation;
		
		temp.GetComponent<Text> ().text = dmg.ToString ();
		Destroy(temp.gameObject, 2); 
	}

    public void ApplyEffect(Effect eff)
    {
        myEffects.Add(eff);
    }

	public bool IsStunned() {
		foreach (Effect eff in myEffects) {
			if (eff.description.Equals("Stun")) {
				return true;
			}
		}
		return false;
	}

	public bool IsSnared() {
		foreach (Effect eff in myEffects) {
			if (eff.description.Equals("Snare")) {
				return true;
			}
		}
		return false;
	}

	public void AddComboPoints(int i) {
		comboPoints += i;
		if (comboPoints > 5) {
			comboPoints = 5;
		}
	}

	public int UseComboPoints() {
		int cp = comboPoints;
		comboPoints = 0;
		return cp;
	}
}
