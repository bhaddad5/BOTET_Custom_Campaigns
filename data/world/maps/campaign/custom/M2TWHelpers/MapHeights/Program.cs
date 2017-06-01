using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

class Program
{
	static Bitmap mapHeightsIN;
	static Bitmap mapHeightsOUT;
	static Color mhBaseWater = Color.FromArgb(255, 0, 0, 255);
	static Color mhCoastCornerWater = Color.FromArgb(255, 0, 0, 235);

	static Bitmap mapGroundTypesOUT;
	static Color gtWater = Color.FromArgb(255, 255, 0, 0);
	static Color gtCoast = Color.FromArgb(255, 255, 255, 255);
	static Color gtWilderness = Color.FromArgb(255, 0, 0, 0);
	static Color gtHills = Color.FromArgb(255, 128, 128, 64);
	static Color gtLowMountains = Color.FromArgb(255, 98, 65, 65);
	static Color gtHighMountains = Color.FromArgb(255, 64, 64, 64);
	static Color gtPeakMountains = Color.FromArgb(255, 196, 128, 128);

	static Bitmap mapClimatesOUT;
	static Color mcOcean = Color.FromArgb(255, 0, 0, 249);
	static Color mcBaseClimate = Color.FromArgb(255, 237, 28, 36);

	static Bitmap mapRegionsOut;
	static Color mrOceanRegion = Color.FromArgb(255, 41, 140, 235);
	static Color mrBaseRegion = Color.FromArgb(255, 73, 163, 33);


	static void Main(string[] args)
	{
		//Console.WriteLine("Hello Norsca");
		//Console.ReadLine();
		mapHeightsIN = new Bitmap("map_heights.png");
		mapHeightsOUT = new Bitmap(mapHeightsIN.Width, mapHeightsIN.Height);
		mapGroundTypesOUT = new Bitmap(mapHeightsIN.Width, mapHeightsIN.Height);
		mapClimatesOUT = new Bitmap(mapHeightsIN.Width, mapHeightsIN.Height);
		mapRegionsOut = new Bitmap(mapHeightsIN.Width/2, mapHeightsIN.Height/2);
		for (int i = 0; i < mapHeightsIN.Width; i++)
		{
			for(int j = 0; j < mapHeightsIN.Height; j++)
			{
				ProcessPixelHeights(i, j);
			}
		}
		for (int i = 0; i < mapHeightsIN.Width; i++)
		{
			for (int j = 0; j < mapHeightsIN.Height; j++)
			{
				ProcessPixelGroundTypes(i, j);
			}
		}
		for (int i = 0; i < mapHeightsIN.Width; i++)
		{
			for (int j = 0; j < mapHeightsIN.Height; j++)
			{
				ProcessPixelClimates(i, j);
			}
		}
		for (int i = 0; i < mapHeightsIN.Width/2; i++)
		{
			for (int j = 0; j < mapHeightsIN.Height/2; j++)
			{
				ProcessPixelRegions(i, j);
			}
		}

		mapHeightsOUT.Save("map_heights_NEW.png");
		mapGroundTypesOUT.Save("map_ground_types_NEW.png");
		mapClimatesOUT.Save("map_climates_NEW.png");
		mapRegionsOut.Save("map_regions_NEW.png");
		Console.WriteLine("done");
		Console.ReadLine();
	}

	private static void ProcessPixelHeights(int i, int j)
	{
		int diagLandTiles = 0;
		foreach (Color adj in GetDiagonalsIN(i, j))
		{
			if (mhIsLand(adj))
				diagLandTiles++;
		}
		int adjacentLandTiles = 0;
		foreach (Color adj in GetAdjacentsIN(i, j))
		{
			if (mhIsLand(adj))
				adjacentLandTiles++;
		}

		if (mhIsLand(mapHeightsIN.GetPixel(i, j)))
		{
			if (adjacentLandTiles == 1)
				mapHeightsOUT.SetPixel(i, j, mhBaseWater);
			else mapHeightsOUT.SetPixel(i, j, mapHeightsIN.GetPixel(i, j));
		}
		if(mhIsWater(mapHeightsIN.GetPixel(i, j)))
		{
			if (diagLandTiles > 0 && adjacentLandTiles == 0)
				mapHeightsOUT.SetPixel(i, j, mhCoastCornerWater);
			else mapHeightsOUT.SetPixel(i, j, mhBaseWater);
		}
	}

	private static bool mhIsWater(Color c)
	{
		return c.B > c.R;
	}

	private static bool mhIsLand(Color c)
	{
		return c.B == c.R && c.B == c.G;
	}

	private static List<Color> GetAdjacentsIN(int i, int j)
	{
		List<Color> adjacents = new List<Color>();
		if (GetColor(i + 1, j) != null)
			adjacents.Add(GetColor(i + 1, j).Value);
		if (GetColor(i - 1, j) != null)
			adjacents.Add(GetColor(i - 1, j).Value);
		if (GetColor(i, j - 1) != null)
			adjacents.Add(GetColor(i, j - 1).Value);
		if (GetColor(i, j + 1) != null)
			adjacents.Add(GetColor(i, j + 1).Value);
		return adjacents;
	}

	private static List<Color> GetDiagonalsIN(int i, int j)
	{
		List<Color> diags = new List<Color>();
		if(GetColor(i + 1, j + 1) != null)
			diags.Add(GetColor(i + 1, j + 1).Value);
		if (GetColor(i - 1, j - 1) != null)
			diags.Add(GetColor(i - 1, j - 1).Value);
		if (GetColor(i + 1, j - 1) != null)
			diags.Add(GetColor(i + 1, j - 1).Value);
		if (GetColor(i - 1, j + 1) != null)
			diags.Add(GetColor(i - 1, j + 1).Value);
		return diags;
	}

	private static Color? GetColor(int i, int j)
	{
		if (i >= 0 && i < mapHeightsIN.Width &&
			j >= 0 && j < mapHeightsIN.Height)
			return mapHeightsIN.GetPixel(i, j);
		else return null;
	}

	private static void ProcessPixelGroundTypes(int i, int j)
	{
		if (gtShouldBeOcean(i, j))
			mapGroundTypesOUT.SetPixel(i, j, gtWater);
		else if (gtShouldBeCoast(i, j))
			mapGroundTypesOUT.SetPixel(i, j, gtCoast);
		else if (gtShouldBePeakMountains(i, j))
			mapGroundTypesOUT.SetPixel(i, j, gtPeakMountains);
		else if (gtShouldBeLowMountains(i, j))
			mapGroundTypesOUT.SetPixel(i, j, gtLowMountains);
		else if (gtShouldBeHighMountains(i, j))
			mapGroundTypesOUT.SetPixel(i, j, gtHighMountains);
		else if (gtShouldBeHills(i, j))
			mapGroundTypesOUT.SetPixel(i, j, gtHills);
		else mapGroundTypesOUT.SetPixel(i, j, gtWilderness);
	}

	private static bool gtShouldBeOcean(int i, int j)
	{
		return mhIsWater(mapHeightsOUT.GetPixel(i, j));
	}

	private static bool gtShouldBeCoast(int i, int j)
	{
		if (mhIsWater(mapHeightsOUT.GetPixel(i, j)))
			return false;
		int adjacentWaterTiles = 0;
		foreach (Color adj in GetAdjacentsIN(i, j))
		{
			if (mhIsWater(adj))
				adjacentWaterTiles++;
		}
		return adjacentWaterTiles > 0;
	}

	private static bool gtShouldBeHills(int i, int j)
	{
		if (mhIsFlat(i, j))
			return false;
		Color c = mapHeightsOUT.GetPixel(i, j);
		if (mhIsWater(c))
			return false;
		return c.R >= 20 && c.R <= 55;
	}

	private static bool gtShouldBeLowMountains(int i, int j)
	{
		if (mhIsFlat(i, j))
			return false;
		Color c = mapHeightsOUT.GetPixel(i, j);
		if (mhIsWater(c))
			return false;
		return c.R >= 56 && c.R <= 90;
	}

	private static bool gtShouldBeHighMountains(int i, int j)
	{
		if (mhIsFlat(i, j))
			return false;
		Color c = mapHeightsOUT.GetPixel(i, j);
		if (mhIsWater(c))
			return false;
		return c.R >= 90;
	}

	private static bool gtShouldBePeakMountains(int i, int j)
	{
		Color c = mapHeightsOUT.GetPixel(i, j);
		if (mhIsWater(c) || !(c.R >= 155) || mhIsFlat(i, j))
			return false;
		bool highestPoint = true;
		var allAdjacent = GetAdjacentsIN(i, j).Concat(GetDiagonalsIN(i, j));
		foreach(var adj in allAdjacent)
		{
			if (adj.R >= c.R)
				highestPoint = false;
		}
		return highestPoint;
	}

	private static bool mhIsFlat(int i, int j)
	{
		Color c = mapHeightsOUT.GetPixel(i, j);
		int maxHeightDiff = 12;
		bool isFlat = true;
		var allAdjacent = GetAdjacentsIN(i, j).Concat(GetDiagonalsIN(i, j));
		foreach (var adj in allAdjacent)
		{
			if (adj.R > c.R + maxHeightDiff || adj.R < c.R - maxHeightDiff)
				isFlat = false;
		}
		return isFlat;
	}

	private static void ProcessPixelClimates(int i, int j)
	{
		if (gtShouldBeOcean(i, j))
			mapClimatesOUT.SetPixel(i, j, mcOcean);
		else mapClimatesOUT.SetPixel(i, j, mcBaseClimate);
	}

	private static void ProcessPixelRegions(int i, int j)
	{
		bool isOcean = true;
		foreach(Color c in GetPixelBlock(i, j))
		{
			if (mhIsLand(c))
				isOcean = false;
		}
		if (isOcean)
			mapRegionsOut.SetPixel(i, j, mrOceanRegion);
		else mapRegionsOut.SetPixel(i, j, mrBaseRegion);
	}

	private static List<Color> GetPixelBlock(int i, int j)
	{
		var block = new List<Color>();
		block.Add(mapHeightsOUT.GetPixel(i * 2, j * 2));
		block.Add(mapHeightsOUT.GetPixel(i * 2 + 1, j * 2));
		block.Add(mapHeightsOUT.GetPixel(i * 2, j * 2 + 1));
		block.Add(mapHeightsOUT.GetPixel(i * 2 + 1, j * 2 + 1));
		return block;
	}
}

