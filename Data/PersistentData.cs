using System;
using System.Collections.Generic;

[Serializable]
public class PersistentData
{
   public List<Result> Results = [];
   
   public override string ToString()
   {
      return string.Join(": ", Results);
   }
}