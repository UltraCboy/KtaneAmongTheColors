using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crew : MonoBehaviour {

	public Animator crewAnimator;
	public UnityEngine.UI.Text name;
	public KMAudio audio;
	
	public bool isHighlighted = false;
	public int iCrew = 0;
	private string[] nColors = new string[] {"White", "Red", "Brown", "Orange", "Yellow", "Lime", "Black", "Green", "Cyan", "Blue", "Purple", "Pink"};
	private string[] fakeColors = new string[] { "Maroon", "Violet", "Indigo", "Banana", "Blanana", "Gray", "Grey", "Magenta", "Tan", "Teal" };
	public Color[] hColors = new Color[12];
	private int[] ghosts = new int[10];
	
	//tags, body, tag col
	public int[][] aCrew = new int[10][];
	
	private UnityEngine.UI.Image CrewShadow;
	private UnityEngine.UI.Image CrewLight;

	private void Start () {
		CrewShadow = transform.GetChild(1).GetComponent<UnityEngine.UI.Image>();
		CrewLight = transform.GetChild(2).GetComponent<UnityEngine.UI.Image>();
		
		if (aCrew[iCrew][0] > 11) {
			name.text = fakeColors[aCrew[iCrew][0] % 12];
		} else {
			name.text = nColors[aCrew[iCrew][0]];
		}
		name.color = hColors[aCrew[iCrew][2]];
		CrewShadow.color = hColors[aCrew[iCrew][1]];
		CrewLight.color = hColors[aCrew[iCrew][1]];
	}

	private void NextCrew () {
		iCrew = (iCrew + 1) % 10;
		crewAnimator.SetInteger("SelectedCrew", iCrew);
		crewAnimator.SetBool("IsNextGhost", ghosts[iCrew] == 1);
		
		if (aCrew[iCrew][0] > 11) {
			name.text = fakeColors[aCrew[iCrew][0] % 12];
		} else {
			name.text = nColors[aCrew[iCrew][0]];
		}
		name.color = hColors[aCrew[iCrew][2]];
		CrewShadow.color = hColors[aCrew[iCrew][1]];
		CrewLight.color = hColors[aCrew[iCrew][1]];
	}
	
	private void AddGhost () {
		ghosts[iCrew] = 1;
	}
	
	private void PlayStep () {
		if (isHighlighted) {
			audio.PlaySoundAtTransform("Footstep" + UnityEngine.Random.Range(1, 8), transform);
		}
	}
}
