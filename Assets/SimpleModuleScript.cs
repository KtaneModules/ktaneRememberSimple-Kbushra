using System.Collections.Generic;
using UnityEngine;
using KModkit;
using Newtonsoft.Json;
using System.Linq;
using System.Text.RegularExpressions;
using Rnd = UnityEngine.Random;

public class SimpleModuleScript : MonoBehaviour {

	public KMAudio audio;
	public KMBombInfo info;
	public KMBossModule BossModule;
	public KMBombModule module;
	public KMSelectable[] button;
	private string[] ignoredModules;

	public int stageSub = 1;
	public int stageCur;
	public int StagesTotes;

	public bool _isSolved = false;

	public AudioSource correct;

	static int ModuleIdCounter;
	int ModuleId;

	void Awake()
	{
		ModuleId = ModuleIdCounter++;

		foreach (KMSelectable button in button)
		{
			KMSelectable pressedButton = button;
			button.OnInteract += delegate () { buttonPress(pressedButton); return false; };
		}
	}

	void Start()
	{
		if (ignoredModules == null) 
		{
			ignoredModules = BossModule.GetIgnoredModules ("Remember Simple", new string[] 
			{
					"14",
					"Cruel Purgatory",
					"Forget Enigma",
					"Forget Everything",
					"Forget It Not",
					"Forget Infinity",
					"Forget Me Later",
					"Forget Me Not",
					"Forget Perspective",
					"Forget Them All",
					"Forget This",
					"Forget Us Not",
					"Organization",
					"Purgatory",
					"Simon's Stages",
					"Souvenir",
					"Tallordered Keys",
					"The Time Keeper",
					"Timing is Everything",
					"The Troll",
					"Turn The Key",
					"Übermodule",
					"Ültimate Custom Night",
					"The Very Annoying Button",
					"Remember Simple"
			});

			module.OnActivate += delegate () { 
				StagesTotes = info.GetSolvableModuleNames().Where(a => !ignoredModules.Contains(a)).ToList().Count;
				if (StagesTotes > 0)
				{
					Log("Yes Stages");
				}
				else
				{
					Log("No Stages");
					module.HandlePass();
				}
			};
		}
	}

	void FixedUpdate()
	{
		stageCur = info.GetSolvedModuleNames().Where(a => !ignoredModules.Contains(a)).ToList().Count;

		if (stageSub < stageCur) 
		{
			Invoke ("TooLittle", 0);
		}
		if (stageSub > stageCur + 1) 
		{
			Invoke ("TooMany", 0);
		}
	}

	void TooLittle()
	{
		module.HandleStrike ();
		stageSub = stageCur + 1;
		Debug.Log(string.Format("You didnt press the button! Submitted stage presses were {0}", stageSub));
	}

	void TooMany()
	{
		module.HandleStrike ();
		stageSub = stageCur + 1;
		Debug.Log(string.Format("Too many times! Submitted stage presses were {0}", stageSub));
	}

	public void buttonPress(KMSelectable pressedButton)
	{
		GetComponent<KMAudio>().PlayGameSoundAtTransformWithRef(KMSoundOverride.SoundEffect.ButtonPress, transform);
		int buttonPosition = new int();
		for(int i = 0; i < button.Length; i++)
		{
			if (pressedButton == button[i])
			{
				buttonPosition = i;
				break;
			}
		}

		if (_isSolved == false) 
		{
			switch (buttonPosition) 
			{
			case 0:
				stageSub++;
				Log ("Stage submitted input? Done!");
				correct.Play ();

				if (stageSub == StagesTotes + 1) 
				{
					module.HandlePass ();
					Log ("This button has fooled you long enough");
				}
				break;
			}
		}
	}



	void Log(string message)
	{
		Debug.LogFormat("[Black Screens #{0}] {1}", ModuleId, message);
	}
}
