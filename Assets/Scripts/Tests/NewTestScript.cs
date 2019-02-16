using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class NewTestScript 
{
	public bool EQ(Vector3 a, Vector3 b)
	{
		return Mathf.Approximately(a.x, b.x) &&
			   Mathf.Approximately(a.y, b.y) &&
			   Mathf.Approximately(a.z, b.z);
	}

    [Test]
    public void NewTestScriptSimplePasses() 
	{
		var vec = new Vector3(1f, 0f, 4f);
		var mat = Matrix4x4.LookAt(new Vector3(-1f, 0f, 0f), new Vector3(-1f, 0f, 1f), Vector3.up);

		UnityEngine.Debug.Log(mat);

		var tr = mat.MultiplyPoint(vec);

		Assert.IsTrue(EQ(tr, new Vector3(0f, 0f, 4f)), tr.ToString());
    }
}
