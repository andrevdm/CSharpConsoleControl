using System;

[AttributeUsage(AttributeTargets.Assembly, Inherited=false)]
public sealed class AssemblyFileVersionAttribute : Attribute
{
   public AssemblyFileVersionAttribute(string version)
   {
   }

    // Properties
    public string Version { get; set; }
}

 
