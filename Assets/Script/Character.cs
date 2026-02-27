using UnityEngine;
using System.Collections;

public class Character {

    /// <summary>
    /// 装备类型枚举
    /// </summary>
    public enum EquipmentType
    {
        Head = 0,
        Chest = 1,
        Hand = 2,
        Feet = 3
    }

    /// <summary>
    /// GameObject reference
    /// </summary>
	public GameObject skeletonInstance = null;
	public GameObject weaponInstance = null;

    /// <summary>
    /// Equipment informations
    /// </summary>
	public string skeletonName;
	public string equipmentHeadName;
	public string equipmentChestName;
	public string equipmentHandName;
	public string equipmentFeetName;

    /// <summary>
    /// The unique id in the scene
    /// </summary>
	public int characterIndex;

    /// <summary>
    /// Other vars
    /// </summary>
	public bool rotate = false;
	public int animationState = 0;

	private Animation animationController = null;

	public Character (int index, string skeleton, string weapon, string head, string chest, string hand, string feet, bool combine = false) {

		//Creates the skeletonName object
		Object res = Resources.Load ("Prefab/" + skeleton);
		this.skeletonInstance = GameObject.Instantiate (res) as GameObject;
		this.characterIndex = index;
		this.skeletonName = skeleton;
		this.equipmentHeadName = head;
		this.equipmentChestName = chest;
		this.equipmentHandName = hand;
		this.equipmentFeetName = feet;
		
		string[] equipments = new string[4];
		equipments [0] = head;
		equipments [1] = chest;
		equipments [2] = hand;
		equipments [3] = feet;
		
        // Create and collect other parts SkinnedMeshRednerer
		SkinnedMeshRenderer[] meshes = new SkinnedMeshRenderer[4];
		GameObject[] objects = new GameObject[4];
		for (int i = 0; i < equipments.Length; i++) {
			
			res = Resources.Load ("Prefab/" + equipments [i]);
			objects[i] = GameObject.Instantiate (res) as GameObject;
			meshes[i] = objects[i].GetComponentInChildren<SkinnedMeshRenderer> ();
		}
		
        // Combine meshes
		CombineSkinnedHelper.CombineToSkeletonObject(skeletonInstance, meshes, combine);

        // Delete temporal resources
        for (int i = 0; i < objects.Length; i++) {
			GameObject.DestroyImmediate(objects[i].gameObject);
		}
		
		// Create weapon
		res = Resources.Load ("Prefab/" + weapon);
		weaponInstance = GameObject.Instantiate (res) as GameObject;
		
		Transform[] transforms = skeletonInstance.GetComponentsInChildren<Transform>();
		foreach (Transform joint in transforms) {
			if (joint.name == "weapon_hand_r") {// find the joint (need the support of art designer)
				weaponInstance.transform.parent = joint.gameObject.transform;
				break;
			}	
		}

        // Init weapon relative informations
		weaponInstance.transform.localScale = Vector3.one;
		weaponInstance.transform.localPosition = Vector3.zero;
		weaponInstance.transform.localRotation = Quaternion.identity;

        // Only for display
		animationController = skeletonInstance.GetComponent<Animation>();
		PlayStand();
	}

	public void ChangeHeadEquipment (string equipment,bool combine = false)
	{
		ChangeEquipment (EquipmentType.Head, equipment, combine);
	}
	
	public void ChangeChestEquipment (string equipment,bool combine = false)
	{
		ChangeEquipment (EquipmentType.Chest, equipment, combine);
	}
	
	public void ChangeHandEquipment (string equipment,bool combine = false)
	{
		ChangeEquipment (EquipmentType.Hand, equipment, combine);
	}
	
	public void ChangeFeetEquipment (string equipment,bool combine = false)
	{
		ChangeEquipment (EquipmentType.Feet, equipment, combine);
	}
	
	public void ChangeWeapon (string weapon)
	{
		Object res = Resources.Load ("Prefab/" + weapon);
		GameObject oldWeapon = weaponInstance;
		weaponInstance = GameObject.Instantiate (res) as GameObject;
		weaponInstance.transform.parent = oldWeapon.transform.parent;
		weaponInstance.transform.localPosition = Vector3.zero;
		weaponInstance.transform.localScale = Vector3.one;
		weaponInstance.transform.localRotation = Quaternion.identity;
		
		GameObject.Destroy(oldWeapon);
	}
	
	public void ChangeEquipment (EquipmentType equipmentType, string equipment, bool combine = false)
	{
		switch (equipmentType) {
			
		case EquipmentType.Head:
			equipmentHeadName = equipment;
			break;
		case EquipmentType.Chest:
			equipmentChestName = equipment;
			break;
		case EquipmentType.Hand:
			equipmentHandName = equipment;
			break;
		case EquipmentType.Feet:
			equipmentFeetName = equipment;
			break;
		}
		
		string[] equipments = new string[4];
		equipments [0] = equipmentHeadName;
		equipments [1] = equipmentChestName;
		equipments [2] = equipmentHandName;
		equipments [3] = equipmentFeetName;
		
		Object res = null;
		SkinnedMeshRenderer[] meshes = new SkinnedMeshRenderer[4];
		GameObject[] objects = new GameObject[4];
		for (int i = 0; i < equipments.Length; i++) {
			
			res = Resources.Load ("Prefab/" + equipments [i]);
			objects[i] = GameObject.Instantiate (res) as GameObject;
			meshes[i] = objects[i].GetComponentInChildren<SkinnedMeshRenderer> ();
		}
		
		CombineSkinnedHelper.CombineToSkeletonObject(skeletonInstance, meshes, combine);

        for (int i = 0; i < objects.Length; i++) {
			GameObject.DestroyImmediate(objects[i].gameObject);
		}
	}

	public void PlayStand () {

		animationController.wrapMode = WrapMode.Loop;
		animationController.Play("breath");
		animationState = 0;
	}
	
	public void PlayAttack () {
		
		animationController.wrapMode = WrapMode.Once;
		animationController.PlayQueued("attack1");
		animationController.PlayQueued("attack2");
		animationController.PlayQueued("attack3");
		animationController.PlayQueued("attack4");
		animationState = 1;
	}
	
	// Update is called once per frame
	public void Update () {
	
		if (animationState == 1)
		{
			if (! animationController.isPlaying)
			{
				PlayAttack();
			}
		}
		if (rotate)
		{
			skeletonInstance.transform.Rotate(new Vector3(0,90 * Time.deltaTime,0));
		}
	}
}