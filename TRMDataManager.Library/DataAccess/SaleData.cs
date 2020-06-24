using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using TRMDataManager.Library.Internal.DataAccess;
using TRMDataManager.Library.Models;

namespace TRMDataManager.Library.DataAccess
{
    public class SaleData : ISaleData
    {
        private readonly IProductData _productData;
        private readonly ISqlDataAccess _sql;

        public SaleData(IProductData productData,
            ISqlDataAccess sql)
        {
            _productData = productData;
            _sql = sql;
        }
        public void SaveSale(SaleModel saleInfo, string cashierId)
        {
            //TODO : Make this SOLID/DRY/Better

            // Start filling in the sale detail models we will save to the database
            // Fill in the available information
            List<SaleDetailDBModel> details = new List<SaleDetailDBModel>();
            decimal taxRate = ConfigHelper.GetTaxRate() / 100;

            foreach (SaleDetailModel item in saleInfo.SaleDetails)
            {
                SaleDetailDBModel detail = new SaleDetailDBModel
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                };

                //Get the information about this product
                ProductModel productInfo = _productData.GetProductById(detail.ProductId);
                if (productInfo == null)
                {
                    throw new Exception($"The product Id of { detail.ProductId } could not be found in the database.");
                }

                detail.PurchasePrice = (productInfo.RetailPrice * detail.Quantity);

                if (productInfo.IsTaxable)
                {
                    detail.Tax = (detail.PurchasePrice * taxRate);
                }

                details.Add(detail);
            }

            // Create the sale model

            SaleDBModel sale = new SaleDBModel
            {
                SubTotal = details.Sum(x => x.PurchasePrice),
                Tax = details.Sum(x => x.Tax),
                CashierId = cashierId
            };

            sale.Total = sale.SubTotal + sale.Tax;



            try
            {
                _sql.StartTransaction("TRMData");
                // Save the sale model
                _sql.SaveDataInTransaction("dbo.spSale_Insert", sale);

                // Get the ID from the sale model //we assumed that it has the Id
                sale.Id = _sql.LoadDataInTransaction<int, dynamic>("spSale_Lookup", new { sale.CashierId, sale.SaleDate }).FirstOrDefault();

                // Finish filling in the sale detail models
                foreach (SaleDetailDBModel item in details)
                {
                    item.SaleId = sale.Id;
                    // Save the sale detail models
                    _sql.SaveDataInTransaction("dbo.spSaleDetail_Insert", item);
                }

                _sql.CommitTransaction();
            }
            catch
            {
                _sql.RollbackTransaction();
                throw;
            }

        }

        public List<SaleReportModel> GetSaleReport()
        {

            List<SaleReportModel> output = _sql.LoadData<SaleReportModel, dynamic>("dbo.spSale_SaleReport", new { }, "TRMData");

            return output;
        }
    }
}
