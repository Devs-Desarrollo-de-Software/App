using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace TurisGo.Destinos
{
    public class DestinoAppService :
        CrudAppService
        <
         Destino,
         DestinoDto,
         Guid,
         PagedAndSortedResultRequestDto,
         CreateUpdateDestinoDto>,
         IDestinoAppService
        

    {
        public DestinoAppService(IRepository<Destino, Guid> repository) : base(repository) { }
    }

}
        
