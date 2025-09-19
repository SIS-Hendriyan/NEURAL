using NEURAL.Services.Implementations;

namespace NEURAL.Services.Interfaces
{
    public interface IProdSchedExcelReader
    {
        ProdSchedExcelParseResult Read(Stream excelStream);
    }
}
