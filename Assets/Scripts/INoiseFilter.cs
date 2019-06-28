using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public interface INoiseFilter {

  float Evaluate(Vector3 point);

}
