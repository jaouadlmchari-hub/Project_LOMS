using LOMS_Applications_Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOMS_Applications_DataAccess
{
    public interface IMessageBusClient
    {
        void PublishApplicationCreated(ApplicationCreatedEvent eventModel);
    }
}