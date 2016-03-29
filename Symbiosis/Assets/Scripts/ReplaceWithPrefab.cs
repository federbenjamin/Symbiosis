// using UnityEngine;
// using System.Collections;

// [ExecuteInEditMode]
// public class ReplaceWithPrefab : MonoBehaviour {

//  public GameObject newprefab;
//  public bool go = false;

//  void OnDrawGizmos()
//  {
//      if (!Application.isPlaying)
//      {
//          if (go && newprefab != null)
//          {
//              GameObject prefab = UnityEditor.PrefabUtility.InstantiatePrefab(newprefab) as GameObject;
//              prefab.transform.position = transform.position;
//              prefab.transform.rotation = transform.rotation;
//              prefab.transform.parent = transform.parent;
//              DestroyImmediate(gameObject);
//          }
//      }
//  }


// }