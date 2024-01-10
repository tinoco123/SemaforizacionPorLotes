using SemaforoPorLotes.Repository;
using SpreadsheetLight;
using System.IO;
using System.Text;

namespace SemaforizacionPorLotes.Utils
{
    public class AjusteCantidadInicial
    {
        private static string filePath = @"C:\Users\Jonathan Hernandez\Desktop\CantidadesIniciales.xlsx";
        public static void ajustarCantidadInicial()
        {
            int iRow = 2;
            SLDocument sl = new SLDocument(filePath);
            ItemRepositoryImpl itemRepositoryImpl = new ItemRepositoryImpl();
            LotNumberRepositoryImpl lotNumberRepositoryImpl = new LotNumberRepositoryImpl();
            StringBuilder sb = new StringBuilder();
            while (!string.IsNullOrEmpty(sl.GetCellValueAsString(iRow, 1)))
            {
                string item = sl.GetCellValueAsString(iRow, 1);
                string lotNumber = sl.GetCellValueAsString(iRow, 2);
                int initialQuantity = sl.GetCellValueAsInt32(iRow, 3);
                
                int idItem = itemRepositoryImpl.GetItemId(item);
                int idLotNumber = lotNumberRepositoryImpl.GetLotNumberId(idItem, lotNumber);

                if (idItem == -1 || idLotNumber == -1)
                {
                    sb.AppendLine($"{item}, {lotNumber}, {initialQuantity}");
                }
                else
                {
                    // Update lotnumber set quantity and initialQuantity where id = lotnumber id
                    lotNumberRepositoryImpl.UpdateLotNumberQuantity(idLotNumber, initialQuantity, initialQuantity);
                }

                iRow++;
            }
            File.WriteAllText("./FALTANTES.txt", sb.ToString());
        }

    }
}
