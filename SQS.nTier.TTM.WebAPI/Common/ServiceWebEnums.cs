﻿/******************************************************************************
 *                          © 2017 SQS India                            *
 *                          All Rights Reserved.                              *
 *                                                                            *
 ******************************************************************************
 *
 * Modification History:
 * 
 * AKS 18Sep2017 Created the class
 *******************************************************************************/

using System.ComponentModel;

namespace SQS.nTier.TTM.WebAPI.Common
{
    public enum MailType : short
    {
        Welcome = 0,
        ForgotPassword = 1,
        PasswordReset = 2,
        UserAssignment = 3,
        UserActivation = 4,
        TSOUpdation = 5
    }

    public enum UserTypeEnum : short
    {
        Admin = 1,
        Demo = 2,
        Internal = 3,
        Registered = 4,
    }

    public enum TSOStatusEnum : short
    {
        [Description("In Progress")]
        InProgress = 1,

        [Description("Closed")]
        Closed = 2,

        [Description("On Hold")]
        OnHold = 3,

        [Description("Cancelled")]
        Cancelled = 4
    }

    public enum TSRStatusEnum : short
    {
        [Description("In Progress")]
        InProgress = 1,

        [Description("Closed")]
        Closed = 2,

        [Description("On Hold")]
        OnHold = 3,

        [Description("Cancelled")]
        Cancelled = 4
    }
}