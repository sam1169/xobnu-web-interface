using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CorporateContacts.Domain.Abstract;
using CorporateContacts.Domain.Concrete;
using CorporateContacts.WebUI.Infrastructure.Concrete;
using Ninject;

namespace CorporateContacts.WebUI.Infrastructure
{
    public class NinjectControllerFactory : DefaultControllerFactory
    {
        private IKernel ninjectKernel;
        public NinjectControllerFactory()
        {
            ninjectKernel = new StandardKernel();
            AddBindings();
        }

        protected override IController GetControllerInstance(System.Web.Routing.RequestContext requestContext, Type controllerType)
        {
            return controllerType == null ? null : (IController)ninjectKernel.Get(controllerType);
        }

        private void AddBindings()
        {
            ninjectKernel.Bind<IPlanRepo>().To<EFPlanRepo>();
            ninjectKernel.Bind<IFeatureRepo>().To<EFFeatureRepo>();
            ninjectKernel.Bind<IAccountRepo>().To<EFAccountRepo>();
            ninjectKernel.Bind<IUserRepo>().To<EFUserRepo>();
            ninjectKernel.Bind<IPurchasedFeatureRepo>().To<EFPurchasedProdRepo>();
            ninjectKernel.Bind<INotificationManager>().To<EmailNotificationManager>();
            ninjectKernel.Bind<IAuthProvider>().To<DBAuthProvider>();
            ninjectKernel.Bind<IFolderRepo>().To<EFFolderRepo>();
            ninjectKernel.Bind<IFolderFieldRepo>().To<EFFolderFieldRepo>();
            ninjectKernel.Bind<ICredentialRepo>().To<EFCredentialRepo>();           
            ninjectKernel.Bind<ICCFolderRepo>().To<EFCCFolderRepo>();
            ninjectKernel.Bind<ICCFolderFieldRepo>().To<EFCCFolderFieldRepo>();
            ninjectKernel.Bind<ICCItemRepo>().To<EFCCItemRepo>();
            ninjectKernel.Bind<ICCFieldValuesRepo>().To<EFCCFieldValueRepo>();
            ninjectKernel.Bind<ICCGroupRepo >().To<EFCCGroupRepo>();
            ninjectKernel.Bind<ICCGroupFieldRepo >().To<EFCCGroupFieldRepo>();
            ninjectKernel.Bind<ICCLayoutRepo>().To<EFLayoutRepo>();
            ninjectKernel.Bind<ICCLayoutGroupRepo >().To<EFCCLayoutGroupRepo>();
            ninjectKernel.Bind<ICCConnectionsRepo>().To<EFCCConnectionsRepo>();
            ninjectKernel.Bind<ICCFieldMappingsRepo>().To<EFCCFieldMappingRepo>();
            ninjectKernel.Bind<ICCNoteRepo>().To<EFCCNoteRepo>();
            ninjectKernel.Bind<ICCTokenRepo>().To<EFCCTokenRepo>();
            ninjectKernel.Bind<ICCSyncFieldsRepo>().To<EFCCSyncField>();
            ninjectKernel.Bind<ICCSyncItems>().To<EFCCSyncItems>();
            ninjectKernel.Bind<ICCErrorLogRepo>().To<EFCCErrorLogRepo>();
            ninjectKernel.Bind<ICCLogonLogRepo>().To<EFCCLogonLogRepo>();
            ninjectKernel.Bind<ICCHistoryLogRepo>().To<EFCCHistoryLogRepo>();
            ninjectKernel.Bind<ICCHealthMsgs>().To<EFCCHealthMsgs>();
        }
    }
}