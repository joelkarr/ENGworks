//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CDWKS.Model.Poco.Content
{
    [DataContract]
    public class ContentLibrary
    {
        [DataMember]
        public  int Id
        {
            get;
            set;
        }
        [DataMember]
        public  string Name
        {
            get;
            set;
        }
        [DataMember]
        public ICollection<TreeNode> TreeNodes { get; set; }
    	
    	
    }
}
