﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CorporateContacts.Domain.Entities;

namespace CorporateContacts.Domain.Abstract
{
    public interface ICredentialRepo
    {
        void SetConnectionString(string connStr);

        IQueryable<Credential> Credentials { get; set; }

        Credential SaveCredential(Credential cred);
     

        bool DeleteCredential(long CredentialId);
    }
}
