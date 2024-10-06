using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KModkit;

public class amongTheColors : MonoBehaviour {
	
	public KMBombModule module;
    public KMBombInfo bombInfo;
    public KMAudio audio;
	public KMSelectable selectable;
	
	private static int moduleIdCounter = 1;
	private int moduleId;
	private bool isSolved;
	
	private bool isPlaying = false;
	
	private int[] iColors = new int[] {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11}; 
	private string[] nColors = new string[] {"White", "Red", "Brown", "Orange", "Yellow", "Lime", "Black", "Green", "Cyan", "Blue", "Purple", "Pink"};
	private string[] fakeColors = new string[] { "Maroon", "Violet", "Indigo", "Banana", "Blananas", "Gray", "Grey", "Magenta", "Tan", "Teal" };
	private int[][] Crew = new int[10][];
	private int offset = 0;
	private int offset2 = 0;
	private int impostor = 99;
	private int[] ghosts = new int[10];

	public Crew ScriptCrew;
	public KMSelectable Eject;
	public UnityEngine.UI.Text CheckText;

	void Start () {
		offset = bombInfo.GetBatteryCount(Battery.AA) - bombInfo.GetBatteryCount(Battery.D);
		Debug.LogFormat("[Among the Colors #{0}] After batteries: {1}", moduleId, offset);
		offset += (bombInfo.GetPortCount(Port.Parallel) * 2) + (bombInfo.GetPortCount(Port.PS2) * 2);
		Debug.LogFormat("[Among the Colors #{0}] After Para PS2: {1}", moduleId, offset);
		offset += (bombInfo.GetPortCount(Port.StereoRCA) * 4);
		Debug.LogFormat("[Among the Colors #{0}] After RCA: {1}", moduleId, offset);
		offset -= (bombInfo.GetPortCount(Port.Serial) * 2) + (bombInfo.GetPortCount(Port.RJ45) * 2);
		Debug.LogFormat("[Among the Colors #{0}] After Serial RJ: {1}", moduleId, offset);
		foreach ( string module in bombInfo.GetModuleNames() ) {
			if ( module.Contains("Color") || module.Contains("Colour") ) offset += 5;
		}
		offset -= 5;
		Debug.LogFormat("[Among the Colors #{0}] After Colors Mods: {1}", moduleId, offset);
		offset2 = offset;
		
		foreach ( int[] c in Crew ) {
			if (c[0] == (c[1]+6) % 12 || c[0] == (c[2]+6) % 12 || c[1] == (c[2]+6) % 12) {
				offset += 3;
			}
		}
		
		Debug.LogFormat("[Among the Colors #{0}] After crew self colors: {1}", moduleId, offset);
		
		for ( int i = 1; i < 10; i++ ) {
			for ( int j = 0; j < 3; j++ ) {
				if (Crew[i][j] == (Crew[i-1][j]+6) % 12) {
					offset -= 3;
					break;
				}
			}
		}
		
		Debug.LogFormat("[Among the Colors #{0}] After crew other colors: {1}", moduleId, offset);
		
		int sColor = mod(offset + Crew[9][2], 12);
		Debug.LogFormat("[Among the Colors #{0}] Offset is {1}, Sus color is {2}", moduleId , offset, nColors[sColor]);
		int [][] Suspects = new int[3][];
		int x = 0;
		foreach ( int[] c in Crew ) {
			if (c[0] == sColor || c[1] == sColor || c[2] == sColor) {
				Suspects[x] = c;
				x++;
				continue;
			}
		}
		
		if ( Suspects[2] == null ) {
			if ( System.Array.IndexOf(Crew, Suspects[1]) == 9) {
				Suspects[2] = Crew[8];
			} else {
				Suspects[2] = Crew[9];
			}
		}
		
		if ( Suspects[1] == null || UnityEngine.Random.value < 0.2f) {
			float choice = UnityEngine.Random.value;
			int ichoice = UnityEngine.Random.Range(0, 10);
			if (choice < 0.25f) {
				impostor = ichoice;
				int cchoice = UnityEngine.Random.Range(0, 12);
				for ( int i = 0; i < 3; i++ ) {
					Crew[ichoice][i] = cchoice;
				}
			} else if (choice < 0.5f) {
				impostor = ichoice;
				Crew[ichoice][0] = UnityEngine.Random.Range(0, 12) + 11;
			} else if (choice < 0.75f) {
				int achoice = UnityEngine.Random.Range(0, 3);
				int crew1 = UnityEngine.Random.Range(0, 3);
				int crew2 = UnityEngine.Random.Range(3, 7);
				int crew3 = UnityEngine.Random.Range(7, 10);
				
				int crew_a = Crew[crew1][achoice];
				Crew[crew2][achoice] = crew_a;
				Crew[crew3][achoice] = crew_a;
				
				Suspects[0] = Crew[crew1];
				Suspects[1] = Crew[crew2];
				Suspects[2] = Crew[crew3];
			} else {
				int achoice = UnityEngine.Random.Range(0, 3);
				int crew1 = UnityEngine.Random.Range(0, 5);
				int crew2 = UnityEngine.Random.Range(5, 9);
				
				int crew_a = Crew[crew1][achoice];
				Crew[crew2][achoice] = crew_a;
				
				Suspects[0] = Crew[crew1];
				Suspects[1] = Crew[crew2];
				Suspects[2] = Crew[9];
			}
		}
		
		for ( int i = 0; i < 10; i++ ) {
			string colorname;
			if ( Crew[i][0] > 11 ) colorname = "Fake Name";
			else colorname = nColors[Crew[i][0]];
			Debug.LogFormat("[Among the Colors #{0}] Crewmate number {1} is {2}, {3}, {4}", moduleId, i+1, colorname, nColors[Crew[i][1]], nColors[Crew[i][2]]);
		} 
		
		if ( Suspects[1] != null ) {
			foreach ( int[] s in Suspects ) {
				Debug.LogFormat("[Among the Colors #{0}] Crewmate {1} is a suspect", moduleId, System.Array.IndexOf(Crew, s) + 1);
				
				if (s[0] == (s[1]+6) % 12 || s[0] == (s[2]+6) % 12 || s[1] == (s[2]+6) % 12) {
					offset2 += 3;
				}
			}
			Debug.LogFormat("[Among the Colors #{0}] After crew self colors, offset 2: {1}", moduleId, offset2);
		
			for ( int i = 1; i < 3; i++ ) {
				for ( int j = 0; j < 3; j++ ) {
					if (Suspects[i][j] == (Suspects[i-1][j]+6) % 12) {
						offset2 -= 3;
						break;
					}
				}
			}
			Debug.LogFormat("[Among the Colors #{0}] After crew other colors, offset 2: {1}", moduleId, offset2);
		}
		
		if (impostor == 99) {
			offset2 = (int)Mathf.Round(offset2/3.0f);
			impostor = System.Array.IndexOf(Crew, Suspects[mod(offset2, 3)]);
			Debug.LogFormat("[Among the Colors #{0}] Second offset is {1}", moduleId, offset2);
		}
		Debug.LogFormat("[Among the Colors #{0}] Crewmate {1} is the impostor.", moduleId, impostor+1);
	}

	void Awake () {
		moduleId = moduleIdCounter++;
		
		Eject.OnInteract += delegate () { checkEject(); return false; };
		selectable.OnFocus += delegate () { setFocus(true); };
		selectable.OnDefocus += delegate () { setFocus(false); };
		
		int[] tags, body, tcol;
		tags = (int[])iColors.Clone();
		body = (int[])iColors.Clone();
		tcol = (int[])iColors.Clone();
		
		Shuffle(tags);
		Shuffle(body);
		Shuffle(tcol);
		for ( int i = 0; i < 10; i++ ) {
			int temp;
			if ( body[i] == tags[i] ) { 
				temp = body[i];
				body[i] = body[11];
				body[11] = temp;
			}
			if ( tcol[i] == body[i] || tcol[i] == tags[i] ) {
				temp = tcol[i];
				tcol[i] = tcol[11];
				tcol[11] = temp;
			}
			
			Crew[i] = new int[] {tags[i], body[i], tcol[i]};
		}
		
		ScriptCrew.aCrew = Crew;
		
	}
	
	void checkEject	() {
		Eject.AddInteractionPunch();
		if (ghosts[ScriptCrew.iCrew] == 1 || isSolved || isPlaying) return;
		isPlaying = true;
		audio.PlaySoundAtTransform("Eject", transform);
		ScriptCrew.crewAnimator.SetBool("ToEject", true);
		string text;
		bool toStrike = false;
		if (ScriptCrew.iCrew != impostor) {
			text = nColors[Crew[ScriptCrew.iCrew][0]] + " was not The Impostor";
			Debug.LogFormat("[Among the Colors #{0}] Ejected Crewmate {1}, which is wrong.", moduleId, ScriptCrew.iCrew + 1);
			toStrike = true;
		} else {
			string name;
			if (Crew[impostor][0] > 11) name = fakeColors[Crew[impostor][0] % 12];
			else name = nColors[Crew[impostor][0]];
			text = name + " was The Impostor";
			Debug.LogFormat("[Among the Colors #{0}] You ejected the impostor.", moduleId);
			audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.CorrectChime, transform);
			isSolved = true;
			module.HandlePass();
		}
		StartCoroutine(Judge(text, toStrike));
	}
	
	private IEnumerator Judge( string str, bool strike ) {
		ghosts[ScriptCrew.iCrew] = 1;
		yield return new WaitForSeconds(2f);
		audio.PlaySoundAtTransform("Eject_text", transform);
		foreach ( char c in str ) {
			CheckText.text += c;
			yield return new WaitForSeconds(0.08f);
		}
		if (strike) {
			module.HandleStrike();
			yield return new WaitForSeconds(1.2f);
			CheckText.text = "";
			ScriptCrew.crewAnimator.SetBool("ToEject", false);
		}
		isPlaying = false;
	}
		
	void setFocus ( bool focus ) {
		ScriptCrew.isHighlighted = focus;
	}

	//do the shuffle
	int[] Shuffle ( int[] arr ) {
		for (int t = 0; t < arr.Length; t++ )
        {
            int tmp = arr[t];
            int r = Random.Range(t, arr.Length);
            arr[t] = arr[r];
            arr[r] = tmp;
        }
		return arr;
	}
	
	int mod(int a, int b) {  return ((a %= b) < 0) ? a+b : a;  }
}
