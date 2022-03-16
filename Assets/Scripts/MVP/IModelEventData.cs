using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IModelEventData<T> : IModelEventData where T: IModel { }

public interface IModelEventData { }
