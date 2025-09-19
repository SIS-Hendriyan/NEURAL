using DocumentFormat.OpenXml.Office.Word;
using NEURAL.Models.Entities;
using NEURAL.Models.ViewModel;
using System.Data;

namespace NEURAL.Models.Request
{
   public sealed record ProdSchedInterventionUploadRequest(
        long ProdSchedHeaderId,
        string MbrVersion,
        string MbrVersionNew,
        int Year,
        int Month
   );
}
