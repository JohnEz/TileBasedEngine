using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public struct Level {
	public string name;
	public int maxSizeX;
	public int maxSizeY;
	public Tile[] tiles;
}

public class LevelLoader {
	List<Level> levels;

	public Level GetLevel( int i) {
		return levels [i];
	}

	public void ReadInFile(TextAsset loadedXml) {
		levels = new List<Level> ();

		XmlDocument xmlDoc = new XmlDocument ();
		xmlDoc.LoadXml (loadedXml.text);
		XmlNodeList levelList = xmlDoc.GetElementsByTagName ("Level");

		foreach (XmlNode levelInfo in levelList) {
			XmlNodeList levelContent = levelInfo.ChildNodes;
			Level newLevel = new Level();

			foreach (XmlNode content in levelContent)
			{
				switch(content.Name) {
				case "LevelName" 	: newLevel.name = content.InnerText;
					break;
				case "Difficulty"	:
					break;
				case "SizeX" 		: newLevel.maxSizeX = int.Parse(content.InnerText);
					break;
				case "SizeY" 		: newLevel.maxSizeY = int.Parse(content.InnerText);
					break;
				case "TileMap" 		: newLevel.tiles = CreateTilesFromText(content.InnerText, newLevel.maxSizeX, newLevel.maxSizeY);
					break;
				}
			}
			levels.Add(newLevel);
		}

	}

	Tile[] CreateTilesFromText(string data, int maxX = 10, int maxY = 10) {
		Tile[] tiles = new Tile[maxX * maxY];
		char[] chars = data.ToCharArray ();

		int counter = 0;

		for (int i = 0; i < chars.Length; ++i)
		{
			switch(chars[i]) {
			case '0' : tiles[counter] = Tile.FLOOR;
				++counter;
				break;
			case '1' : tiles[counter] = Tile.WALL;
				++counter;
				break;
			case '2' : tiles[counter] = Tile.CARPET;
				++counter;
				break;
			default: break;
			}
		}
		return tiles;
	}

}
