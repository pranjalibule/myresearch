/******************************************************************************
 *                          © 2017 SQS India                            *
 *                          All Rights Reserved.                              *
 *                                                                            *
 ******************************************************************************
 *
 * Modification History:
 * 
 * AKS 18Sep2017 Created the class
 *******************************************************************************/

using System;

namespace SQS.nTier.TTM.GenericFramework
{
    /// <summary>
    /// IEntitySummary
    /// </summary>
    public interface IBaseObject
    {
        int ID { get; set; }
        string CreatedBy { get; set; }
        DateTime CreatedOn { get; set; }
        string UpdatedBy { get; set; }
        DateTime UpdatedOn { get; set; }
        int Version { get; set; }
        ObjectSate ObjectSate { get; set; }
    }

    /// <summary>
    /// ObjectSate
    /// </summary>
    public enum ObjectSate
    {
        New = 0,
        Detached = 1,
        Unchanged = 2,
        Added = 3,
        Modified = 4,
        Deleted = 5,
        Manual = 6
    }
}