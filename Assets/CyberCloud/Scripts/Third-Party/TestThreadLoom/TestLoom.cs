using UnityEngine;
using System.Collections;

public class TestLoom : MonoBehaviour {

	public MeshFilter meshF;
	// Use this for initialization
	void Start () {

		ScaleMesh(meshF.mesh, 2f);
	}

	//Scale a mesh on a second thread  
	void ScaleMesh(Mesh mesh, float scale)  
	{  
		//Get the vertices of a mesh  
		var vertices = mesh.vertices;  
		//Run the action on a new thread  开启多线程
		Loom.RunAsync(()=>{  
			//Loop through the vertices  
			for(var i = 0; i < vertices.Length; i++)  
			{  
				//Scale the vertex  
				vertices[i] = vertices[i] * scale;  
			}  
			//Run some code on the main thread  必须在主线程内执行的
			//to update the mesh  
			Loom.QueueOnMainThread(()=>{  
				//Set the vertices  
				mesh.vertices = vertices;  
				//Recalculate the bounds  
				mesh.RecalculateBounds();  
			});
		});  
	} 
}
