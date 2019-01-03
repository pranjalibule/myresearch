﻿/******************************************************************************
 *                          © 2017 SQS India                            *
 *                          All Rights Reserved.                              *
 *                                                                            *
 ******************************************************************************
 *
 * Modification History:
 * 
 * AKS 19Sep2017 Created the class
 *******************************************************************************/

namespace SQS.nTier.TTM.DTO
{
    using System;

    public class LoginDTO
    {
        public String UserName { get; set; }
        public String Password { get; set; }
        public Boolean ADUser { get; set; }
        public int? roleId { get; set; }
    }
}
