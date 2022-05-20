using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

[System.Serializable]
public class RandomTileColor {

	[System.NonSerialized]
	public Color[] colors = {

        new Color32( 0xFF, 0xFF, 0xFF, 0xFF ), //1
        new Color32( 0xFA, 0xE8, 0x01, 0xFF ), //2
        new Color32( 0xDD, 0x07, 0x07, 0xFF ), //3
        new Color32( 0xFF, 0x3E, 0x00, 0xFF ), //4
        new Color32( 0x90, 0xFD, 0xC5, 0xFF ), //5
        new Color32( 0x00, 0x15, 0xFA, 0xFF ), //6
        new Color32( 0x3B, 0x53, 0x6C, 0xFF ), //7
        new Color32( 0xB8, 0xFD, 0x00, 0xFF ), //8
        new Color32( 0x01, 0x00, 0x67, 0xFF ), //9
        new Color32( 0xE0, 0x7B, 0x7B, 0xFF ), //10

        new Color32( 0x00, 0xAB, 0xA4, 0xFF ), //11
        new Color32( 0x80, 0x00, 0x50, 0xFF ), //12
        new Color32( 0x00, 0x65, 0x1A, 0xFF ), //13
        new Color32( 0xFF, 0x00, 0x99, 0xFF ), //14
        new Color32( 0x00, 0x00, 0x00, 0xFF ), //15
        new Color32( 0x72, 0x29, 0x27, 0xFF ), //16
        new Color32( 0x7B, 0x7B, 0x7B, 0xFF ), //17
        new Color32( 0x37, 0x25, 0x29, 0xFF ), //18
        new Color32( 0x83, 0x00, 0xFF, 0xFF ), //19
        new Color32( 0x27, 0x4D, 0x98, 0xFF ), //20

        new Color32( 0xFF, 0x85, 0x00, 0xFF ), //21
        new Color32( 0x9F, 0x82, 0x00, 0xFF ), //22
        new Color32( 0x00, 0x59, 0x00, 0xFF ), //23
        new Color32( 0xC3, 0x99, 0x99, 0xFF ), //24
        new Color32( 0x8B, 0x58, 0xE7, 0xFF ), //25
    };

	public List<int> randomIndexes;

	public RandomTileColor(int count)
	{
		GenerateRandom(count, 0, 25);		
	}

	public Color AtIndex(int index){
		return colors[randomIndexes.ElementAt(index)];
	}

	public int GetIndex(int index) {
		return randomIndexes.ElementAt(index);
	}

	private void GenerateRandom(int count, int min, int max)
	{
		System.Random random = new System.Random ();		
		HashSet<int> candidates = new HashSet<int>();

		// start count values before max, and end at max
		for (int top = max - count; top < max; top++)
		{
			// May strike a duplicate.
			// Need to add +1 to make inclusive generator
			// +1 is safe even for MaxVal max value because top < max
			if (!candidates.Add(random.Next(min, top + 1))) {
				// collision, add inclusive max.
				// which could not possibly have been added before.
				candidates.Add(top);
			}
		}

		// load them in to a list, to sort
		randomIndexes = candidates.ToList();

		// shuffle the results because HashSet has messed
		// with the order, and the algorithm does not produce
		// random-ordered results (e.g. max-1 will never be the first value)
		for (int i = randomIndexes.Count - 1; i > 0; i--)
		{  
			int k = random.Next(i + 1);  
			int tmp = randomIndexes[k];  
			randomIndexes[k] = randomIndexes[i];  
			randomIndexes[i] = tmp;  
		}  
	}
}
