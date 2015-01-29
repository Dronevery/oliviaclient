using UnityEngine;
using System.Collections;
using AssemblyCSharp;

public interface IDataManager
{
	void UpdateLocation(string name , Location loc);
	void UpdateRotation(string name , Vector3 eulerangle);
}

public class DataManager : MonoBehaviour, IDataManager
{

	// the messenger should only transfer to this datamanager
	// datamanager will diliver the data to all structures
	public IDataPostureSphere _dataposturesphere;

	public void UpdateLocation(string name, Location loc){

	}

	public void UpdateRotation(string name, Vector3 eulerangle){
		_dataposturesphere.UpdateData (name, eulerangle);

	}
}

