using UnityEngine;
using System.Collections;

public class UnityBlendShapeAnimation {

	public UnityCurveContainer[] curves;
	public SkinnedMeshRenderer skinMeshObj;
	string[] blendShapeNames;
	int blendShapeCount = 0;
	public string pathName = "";

	public UnityBlendShapeAnimation( string hierarchyPath, SkinnedMeshRenderer observeSkinnedMeshRenderer ) {
		this.pathName = hierarchyPath;
		this.skinMeshObj = observeSkinnedMeshRenderer;

		//int blendShapeCount = skinMeshObj.getbl
		Mesh blendShapeMesh = this.skinMeshObj.sharedMesh;
		this.blendShapeCount = blendShapeMesh.blendShapeCount;

		this.blendShapeNames = new string[blendShapeCount];
		this.curves = new UnityCurveContainer[this.blendShapeCount];

		// create curve objs and add names
		for (int i = 0; i < this.blendShapeCount; i++) {
			// public string GetBlendShapeName(int shapeIndex);
			this.blendShapeNames [i] = blendShapeMesh.GetBlendShapeName (i);
			this.curves [i] = new UnityCurveContainer ("blendShape." + this.blendShapeNames [i]);
		}
	}

	public void AddFrame ( float time ) {
					
		for (int i = 0; i < this.blendShapeCount; i++)
			this.curves [i].AddValue (time, this.skinMeshObj.GetBlendShapeWeight (i));

	}
}
