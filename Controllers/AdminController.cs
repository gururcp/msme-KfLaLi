using HotelwebLisMVC.Dto;
using HotelwebLisMVC.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Linq.Expressions;
using System.Runtime.Intrinsics.X86;       
using System.Xml.Linq;

namespace HotelwebLisMVC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly HotelWebLisDBContext context;

        public AdminController(HotelWebLisDBContext context)
        {
            this.context = context;

        }


        [HttpPost("new-ledger")]
        public IActionResult PostLedger([FromBody] Ledger request, string Action)
        {
            if (Action == "submit")
            {
                // Check if a ledger with the same name already exists (case-insensitive)
                bool exists = context.Ledgers
                    .Any(l => l.LedgerName.ToLower() == request.LedgerName.ToLower());

                if (exists)
                {
                    return BadRequest("Ledger with the same name already exists.");
                }

                //if (!string.IsNullOrEmpty(request.Gstnumber))
                //{
                //    bool gstExists = context.Ledgers
                //        .Any(l => l.Gstnumber == request.Gstnumber);
                //    if (gstExists) return BadRequest("GST number already exists.");
                //}


                var ledger = new Ledger
                {
                    LedgerName = request.LedgerName,
                    LedgerTypeId = request.LedgerTypeId,
                    Gsttype = request.Gsttype,
                    Gstnumber = request.Gstnumber,
                    MobileNumber = request.MobileNumber,
                    PhoneNumber = request.PhoneNumber,
                    BillingAddress = request.BillingAddress,
                    ShipingAddress = request.ShipingAddress,
                    EmailId = request.EmailId,
                    State = request.State,
                    OpeningBalance = request.OpeningBalance,
                    OpeningBalanceDate = request.OpeningBalanceDate,
                    CreditLimitStatus = request.CreditLimitStatus,
                    CreditLimit = request.CreditLimit
                };

                context.Ledgers.Add(ledger);
                context.SaveChanges();

                return Ok("Ledger Added Successfully!");
            }
            else if (Action == "update")
            {
                var ledger = context.Ledgers.FirstOrDefault(x => x.LedgerId == request.LedgerId);
                if (ledger == null)
                    return NotFound("Ledger not found for update.");

                //if (!string.IsNullOrEmpty(request.Gstnumber))
                //{
                //    bool gstExists = context.Ledgers
                //        .Any(l => l.Gstnumber == request.Gstnumber);
                //    if (gstExists) return BadRequest("GST number already exists.");
                //}



                ledger.LedgerName = request.LedgerName;
                ledger.Gstnumber = request.Gstnumber;
                ledger.MobileNumber = request.MobileNumber;
                ledger.PhoneNumber = request.PhoneNumber;
                ledger.BillingAddress = request.BillingAddress;
                ledger.ShipingAddress = request.ShipingAddress;
                ledger.EmailId = request.EmailId;
                ledger.State = request.State;
                ledger.OpeningBalance = request.OpeningBalance;
                ledger.OpeningBalanceDate = request.OpeningBalanceDate;
                ledger.CreditLimitStatus = request.CreditLimitStatus;
                ledger.CreditLimit = request.CreditLimit;

                context.SaveChanges();
                return Ok("Ledger Updated Successfully!");
            }
            else if (Action == "delete")
            {
                var ledger = context.Ledgers.FirstOrDefault(x => x.LedgerId == request.LedgerId);
                if (ledger == null)
                    return NotFound("Ledger not found for delete.");


                if (ledger.LedgerId == 1 && ledger.LedgerName.ToLower() == "cash")
                {
                    return BadRequest("Cash Ledger cannot be deleted.");
                }

                if (IsIdInUse(context, "LedgerId", request.LedgerId, typeof(Ledger)))
                {
                    return Conflict("Cannot delete this Ledger as it is assigned in one or more other tables.");
                }

                context.Ledgers.Remove(ledger);
                context.SaveChanges();
                return Ok("Ledger Deleted Successfully!");
            }

            return BadRequest("Invalid Action.");
        }



        [HttpPost("add-ledgertype")]
        public ActionResult<string> AddLedgerType([FromBody] LedgerType request)
        {
            // Check if a ledger type with the same name already exists (case-insensitive)
            bool exists = context.LedgerTypes
                .Any(l => l.LedgerTypeName.ToLower() == request.LedgerTypeName.ToLower());

            if (exists)
            {
                return BadRequest("Record already exists.");
            }

            var ledgerType = new LedgerType
            {
                LedgerTypeName = request.LedgerTypeName
            };

            context.LedgerTypes.Add(ledgerType);
            context.SaveChanges();

            return Ok("Ledger Type added successfully!");
        }



        [HttpGet("get-ledgertypes")]
        public ActionResult<IEnumerable<LedgerType>> GetLedgerTypes()
        {
            var ledgerTypes = context.LedgerTypes.ToList();

            if (ledgerTypes == null || !ledgerTypes.Any())
            {
                return NotFound("No ledger types found.");
            }

            return Ok(ledgerTypes);
        }



        //[HttpGet("getledgerdata")]
        //public async Task<IActionResult> GetLedgerData()
        //{
        //    var data = await (
        //        from ledger in context.Ledgers
        //        join ledgertype in context.LedgerTypes on ledger.LedgerTypeId equals ledgertype.LedgerTypeId
        //        where ledger.LedgerTypeId == 1004
        //        orderby ledger.LedgerId descending // 👈 Order by LedgerId descending
        //        select new
        //        {
        //            LedgerId = ledger.LedgerId,
        //            LedgerName = ledger.LedgerName,
        //            LedgerType = ledgertype != null ? ledgertype.LedgerTypeName : null,
        //            Gsttype = ledger.Gsttype,
        //            Gstnumber = ledger.Gstnumber,
        //            MobileNumber = ledger.MobileNumber,
        //            PhoneNumber = ledger.PhoneNumber,
        //            BillingAddress = ledger.BillingAddress,
        //            ShipingAddress = ledger.ShipingAddress,
        //            EmailId = ledger.EmailId,
        //            State = ledger.State,
        //            OpeningBalance = ledger.OpeningBalance,
        //            OpeningBalanceDate = ledger.OpeningBalanceDate,
        //            CreditLimitStatus = ledger.CreditLimitStatus,
        //            CreditLimit = ledger.CreditLimit,
        //            LedgerTypeId = ledger.LedgerTypeId
        //        }
        //    ).ToListAsync();

        //    return Ok(data);
        //}


        [HttpGet("getledgerdata")]
        public async Task<IActionResult> GetLedgerData()
        {
            var data = await (
                from ledger in context.Ledgers
                join ledgertype in context.LedgerTypes on ledger.LedgerTypeId equals ledgertype.LedgerTypeId
                where ledger.LedgerTypeId == 5 // 👈 Add this where condition
                orderby ledger.LedgerId descending
                select new
                {
                    LedgerId = ledger.LedgerId,
                    LedgerName = ledger.LedgerName,
                    LedgerType = ledgertype != null ? ledgertype.LedgerTypeName : null,
                    Gsttype = ledger.Gsttype,
                    Gstnumber = ledger.Gstnumber,
                    MobileNumber = ledger.MobileNumber,
                    PhoneNumber = ledger.PhoneNumber,
                    BillingAddress = ledger.BillingAddress,
                    ShipingAddress = ledger.ShipingAddress,
                    EmailId = ledger.EmailId,
                    State = ledger.State,
                    OpeningBalance = ledger.OpeningBalance,
                    OpeningBalanceDate = ledger.OpeningBalanceDate,
                    CreditLimitStatus = ledger.CreditLimitStatus,
                    CreditLimit = ledger.CreditLimit,
                    LedgerTypeId = ledger.LedgerTypeId
                }
            ).ToListAsync();

            return Ok(data);
        }






        [HttpPost("AddItemCategory")]
        public IActionResult AddItemCategory([FromBody] ItemCategory request, string Action)
        {
            if (Action == "submit")
            {
                // Check if a Category with the same name already exists (case-insensitive)
                bool exists = context.ItemCategories
                    .Any(l => l.CategoryName.ToLower() == request.CategoryName.ToLower());

                if (exists)
                {
                    return BadRequest("Category with the same name already exists.");
                }

                var item = new ItemCategory
                {
                    CategoryName = request.CategoryName
                };

                context.ItemCategories.Add(item);
                context.SaveChanges();

                return Ok("Category Inserted Successfully!");
            }
            else if (Action == "update")
            {
                var item = context.ItemCategories.FirstOrDefault(x => x.CategoryId == request.CategoryId);
                if (item == null) return NotFound("Category not found for update.");

                item.CategoryName = request.CategoryName;

                context.SaveChanges();
                return Ok("Category Updated Successfully!");
            }
            else if (Action == "delete")
            {
                var item = context.ItemCategories.FirstOrDefault(x => x.CategoryId == request.CategoryId);
                if (item == null) return NotFound("Category not found for delete.");


          
                    if (IsIdInUse(context, "CategoryId", item.CategoryId, typeof(ItemCategory)))
                    {
                        return Conflict("Cannot delete this Category as it is referenced in other records.");
                    }

              
                context.ItemCategories.Remove(item);
                context.SaveChanges();
                return Ok("Category Deleted Successfully!");
            }
            return Ok();
        }



        [HttpGet("getCategoryData")]
        public async Task<IActionResult> GetCategoryData()
        {
            var data = await context.ItemCategories
                .Select(c => new
                {
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName
                })
                .ToListAsync();

            return Ok(data);
        }



        [HttpGet("getGstdata")]
        public async Task<IActionResult> GetGstData()
        {
            var data = await context.Gsts
                .Select(e => new
                {
                    GstId = e.GstId,
                    Gstpercentage = e.Gstpercentage,
                    Cgst = e.Cgst,
                    Sgst = e.Sgst,
                    Igst = e.Igst
                })
                .ToListAsync();

            return Ok(data);
        }




        [HttpPost("AddItemUnit")]
        public IActionResult AddItemUnit([FromBody] Unit request, string Action)
        {
            if (Action == "submit")
            {
                // Check if a Category with the same name already exists (case-insensitive)
                bool exists = context.Units
                    .Any(l => l.UnitName.ToLower() == request.UnitName.ToLower());

                if (exists)
                {
                    return BadRequest("Unit with the same name already exists.");
                }

                var unit = new Unit
                {
                    UnitName = request.UnitName
                };

                context.Units.Add(unit);
                context.SaveChanges();

                return Ok("Unit Inserted Successfully!");
            }

            return Ok();
        }




        [HttpGet("getUnitData")]
        public async Task<IActionResult> getUnitData()
        {
            var data = await context.Units
                .Select(c => new
                {
                    UnitId = c.UnitId,
                    UnitName = c.UnitName
                })
                .ToListAsync();

            return Ok(data);
        }


        [HttpGet("getToUnitData")]
        public async Task<IActionResult> GetToUnitData(string fromUnit)
        {
            var data = await context.ComparisonUnits
                .Where(c => c.FromUnit == fromUnit)
                .Select(c => new
                {
                    ToUnit = c.ToUnit,
                    EquivalentValue = c.EquivalentValue
                })
                .ToListAsync();

            return Ok(data);
        }


        [HttpGet("getRawComparisonUnitData")]
        public async Task<IActionResult> getRawComparisonUnitData(string fromunit)
        {
            var data = await context.ComparisonUnits.Where(c => c.FromUnit == fromunit)
                .Select(c => new
                {
                    EquivalentId = c.EquivalentId,
                    Equivalentvalue = c.EquivalentValue
                })
                .ToListAsync();

            return Ok(data);
        }



        //[HttpPost("AddComparisonUnit")]
        //public IActionResult AddComparisonUnit([FromBody] ComparisonUnit request)
        //{


        //    var unit = new ComparisonUnit
        //    {
        //        FromUnit = request.FromUnit,
        //        ToUnit = request.ToUnit,
        //        EquivalentValue = request.EquivalentValue
        //    };

        //    context.ComparisonUnits.Add(unit);
        //    context.SaveChanges();

        //    return Ok("Compariosn Unit Inserted Successfully!");


        //}


        [HttpPost("AddComparisonUnit")]
        public IActionResult AddComparisonUnit([FromBody] ComparisonUnit request)
        {
            // Check if the same combination already exists
            var exists = context.ComparisonUnits.Any(c =>
                c.FromUnit == request.FromUnit &&
                c.ToUnit == request.ToUnit);

            if (exists)
            {
                return BadRequest("Comparison Unit already exists!");
            }

            // Add new entry if not exists
            var unit = new ComparisonUnit
            {
                FromUnit = request.FromUnit,
                ToUnit = request.ToUnit,
                EquivalentValue = request.EquivalentValue
            };

            context.ComparisonUnits.Add(unit);
            context.SaveChanges();

            return Ok("Comparison Unit inserted successfully!");
        }

        


        [HttpGet("getComparisonUnitData")]
        public async Task<IActionResult> getComparisonUnitData()
        {
            var data = await context.ComparisonUnits
                .Select(c => new
                {
                    EquivalentId = c.EquivalentId,
                    FromUnit = c.FromUnit,
                    ToUnit = c.ToUnit,
                    EquivalentValue = c.EquivalentValue
                })
                .ToListAsync();

            return Ok(data);
        }




        [HttpGet("getComparisonUnitById")]
        public IActionResult getComparisonUnitById(int Id)
        {
            var unit = context.ComparisonUnits.FirstOrDefault(x => x.EquivalentId == Id);
            if (unit == null) return NotFound("Comparison Unit not found.");
            return Ok(unit);
        }




        [HttpPut("UpdateComparison/{id}")]
        public async Task<IActionResult> UpdateComparison(int id, [FromBody] ComparisonUnit updatedUnit)
        {
            var unit = context.ComparisonUnits.FirstOrDefault(x => x.EquivalentId == id);
            if (unit == null) return NotFound("Comparison not found for update.");

            unit.FromUnit = updatedUnit.FromUnit;
            unit.ToUnit = updatedUnit.ToUnit;
            unit.EquivalentValue = updatedUnit.EquivalentValue;

            context.SaveChanges();
            return Ok("Compariosn Unit Updated Successfully!");
        }




        [HttpDelete("DeleteCompariosnUnit/{id}")]
        public IActionResult DeleteCompariosnUnit(int id)
        {
            var unit = context.ComparisonUnits.FirstOrDefault(x => x.EquivalentId == id);
            if (unit == null)
                return NotFound("Unit not found for delete.");


            if (IsIdInUse(context, "EquivalentId", id, typeof(ComparisonUnit)))
            {
                return Conflict("Cannot delete this Rawmaterial as it is referenced in other records.");
            }


            context.ComparisonUnits.Remove(unit);
            context.SaveChanges();

            return Ok("Comparison Unit Deleted Successfully!");
        }




        [HttpPost("Item")]
        public IActionResult Item([FromBody] Item request, string Action)
        {
            if (Action == "submit")
            {
                request.ItemId = context.Items.Any() ? context.Items.Max(x => x.ItemId) + 1 : 1;


                // Check if a Item with the same name already exists (case-insensitive)
                bool exists = context.Items
                    .Any(l => l.ItemName.ToLower() == request.ItemName.ToLower());

                if (exists)
                {
                    return BadRequest("Item with the same name already exists.");
                }



                var item = new Item
                {

                    ItemName = request.ItemName,
                    ItemNameOnline = request.ItemNameOnline,
                    CategoryId = request.CategoryId,
                    ItemCode = request.ItemCode,
                    ContainerCharges = request.ContainerCharges,
                    Weight = request.Weight,
                    Unit = request.Unit,
                    Dietary = request.Dietary,
                    Mrpprice = request.Mrpprice,
                    SalePrice = request.SalePrice,
                    PurchasePrice = request.PurchasePrice,
                    GstId = request.GstId,
                    OpeningStock = request.OpeningStock,
                    OpeningStockPrice = request.OpeningStockPrice,
                    OpeningStockDate = request.OpeningStockDate,
                    MinStockToMaintain = request.MinStockToMaintain

                };

                context.Items.Add(item);
                context.SaveChanges();

                return Ok("Item Added Successfully!");
            }
            else if (Action == "update")
            {
                var item = context.Items.FirstOrDefault(x => x.ItemId == request.ItemId);
                if (item == null) return NotFound("Item not found for update.");

                item.ItemName = request.ItemName;
                item.ItemNameOnline = request.ItemNameOnline;
                item.CategoryId = request.CategoryId;
                item.ItemCode = request.ItemCode;
                item.ContainerCharges = request.ContainerCharges;
                item.Weight = request.Weight;
                item.Unit = request.Unit;
                item.Dietary = request.Dietary;
                item.Mrpprice = request.Mrpprice;
                item.SalePrice = request.SalePrice;
                item.PurchasePrice = request.PurchasePrice;
                item.GstId = request.GstId;
                item.OpeningStock = request.OpeningStock;
                item.OpeningStockPrice = request.OpeningStockPrice;
                item.OpeningStockDate = request.OpeningStockDate;
                item.MinStockToMaintain = request.MinStockToMaintain;


                context.SaveChanges();
                return Ok("Item Updated Successfully!");
            }
            else if (Action == "delete")
            {
                var item = context.Items.FirstOrDefault(x => x.ItemId == request.ItemId);
                if (item == null) return NotFound("Item not found for delete.");

                if (IsIdInUse(context, "ItemId", item.ItemId, typeof(Item)))
                {
                    return Conflict("Cannot delete this Item as it is referenced in other records.");
                } 

                context.Items.Remove(item);
                context.SaveChanges();
                return Ok("Item Deleted Successfully!");
            }

            return BadRequest("Invalid Action.");
        }



        //[HttpGet("getItemData")]
        //public async Task<IActionResult> GetItemData()
        //{
        //    var data = await (
        //        from item in context.Items
        //        join category in context.ItemCategories on item.CategoryId equals category.CategoryId
        //        join gst in context.Gsts on item.GstId equals gst.GstId
        //        orderby item.ItemId descending // 👈 Add this line here
        //        select new
        //        {
        //            item.ItemId,
        //            item.ItemName,
        //            item.ItemNameOnline,
        //            item.ItemCode,
        //            item.ContainerCharges,
        //            item.Weight,
        //            item.Unit,
        //            item.Dietary,
        //            item.Mrpprice,
        //            item.SalePrice,
        //            item.PurchasePrice,
        //            item.OpeningStock,
        //            item.OpeningStockPrice,
        //            item.OpeningStockDate,
        //            item.MinStockToMaintain,

        //            Category = new
        //            {
        //                item.CategoryId,
        //                category.CategoryName
        //            },
        //            Gst = new
        //            {
        //                item.GstId,
        //                gst.Gstpercentage,
        //                gst.Cgst,
        //                gst.Sgst,
        //                gst.Igst
        //            }
        //        }
        //    ).ToListAsync();

        //    return Ok(data);
        //}





        [HttpGet("getItemData1")]
        public async Task<IActionResult> GetItemData1()
        {
            var data = await (
                from item in context.Items
                join category in context.ItemCategories on item.CategoryId equals category.CategoryId into categoryJoin
                from category in categoryJoin.DefaultIfEmpty() // 👈 Left join for Category

                join gst in context.Gsts on item.GstId equals gst.GstId into gstJoin
                from gst in gstJoin.DefaultIfEmpty() // 👈 Left join for GST

                orderby item.ItemId descending
                select new
                {
                    item.ItemId,
                    item.ItemName,
                    item.ItemNameOnline,
                    item.ItemCode,
                    item.ContainerCharges,
                    item.Weight,
                    item.Unit,
                    item.Dietary,
                    item.Mrpprice,
                    item.SalePrice,
                    item.PurchasePrice,
                    item.OpeningStock,
                    item.OpeningStockPrice,
                    item.OpeningStockDate,
                    item.MinStockToMaintain,

                    Category = category == null ? null : new
                    {
                        item.CategoryId,
                        category.CategoryName
                    },
                    Gst = gst == null ? null : new
                    {
                        item.GstId,
                        gst.Gstpercentage,
                        gst.Cgst,
                        gst.Sgst,
                        gst.Igst
                    }
                }
            ).ToListAsync();

            return Ok(data);
        }







        [HttpGet("getItemData2")]
        public async Task<IActionResult> GetItemData2()
        {
            var data = await (
                from item in context.Items
                join category in context.ItemCategories on item.CategoryId equals category.CategoryId into categoryJoin
                from category in categoryJoin.DefaultIfEmpty()

                join gst in context.Gsts on item.GstId equals gst.GstId into gstJoin
                from gst in gstJoin.DefaultIfEmpty()

                join recipe in context.Recipes on item.ItemId equals recipe.ItemId into recipeJoin
                from recipe in recipeJoin.DefaultIfEmpty()

                select new
                {
                    item.ItemId,
                    item.ItemName,
                    item.ItemNameOnline,
                    item.ItemCode,
                    item.ContainerCharges,
                    item.Weight,
                    item.Unit,
                    item.Dietary,
                    item.Mrpprice,
                    item.SalePrice,
                    item.PurchasePrice,
                    item.OpeningStock,
                    item.OpeningStockPrice,
                    item.OpeningStockDate,
                    item.MinStockToMaintain,

                    Category = category == null ? null : new
                    {
                        item.CategoryId,
                        category.CategoryName
                    },
                    Gst = gst == null ? null : new
                    {
                        item.GstId,
                        gst.Gstpercentage,
                        gst.Cgst,
                        gst.Sgst,
                        gst.Igst
                    },

                    RecipeId = recipe.RecipeId
                }
            ).OrderByDescending(x => x.ItemId).ToListAsync();

            // 👇 Client-side calculation for FCRCost
            var result = data.Select(d =>
            {
                var recipeDetails = (
                    from rd in context.RecipeDetails
                    join rm in context.RawMaterials on rd.RawMaterialId equals rm.RawMaterialId
                    join pdg in
                        (from pd in context.PurchaseDetails
                         group pd by pd.RawMaterialId into g
                         select new
                         {
                             RawMaterialId = g.Key,
                             TotalPurchaseQuantity = g.Sum(x => x.Quantity),
                             TotalRate = g.Sum(x => x.Rate),
                             AvgRate = g.Average(x => x.Rate),
                             Unit = g.Select(x => x.Unit).FirstOrDefault()
                         }) on rd.RawMaterialId equals pdg.RawMaterialId
                    join cu in context.ComparisonUnits
                        on new { FromUnit = rm.PurchaseUnit, ToUnit = rm.ConsumptionUnit }
                        equals new { cu.FromUnit, cu.ToUnit } into cuJoin
                    from cu in cuJoin.DefaultIfEmpty()
                    where rd.RecipeId == d.RecipeId
                    select new
                    {
                        rd.Quantity,
                        pdg.TotalPurchaseQuantity,
                        pdg.TotalRate,
                       // EquivalentValue = (cu == null ? "1" : cu.EquivalentValue) // 👈 no null-propagation 
                        EquivalentValue = cu != null
                                ? "1 " + cu.FromUnit + " = " + cu.EquivalentValue + " " + cu.ToUnit
                                : null,
                    }
                ).AsEnumerable() // 👈 move to memory before math
                .Select(x => x.EquivalentValue != null && x.TotalPurchaseQuantity != null
                    ? Math.Round(
                        (decimal)(x.Quantity ?? 0) *
                        ((decimal)x.TotalRate / ((decimal)x.TotalPurchaseQuantity * decimal.Parse(x.EquivalentValue))),
                        2
                      )
                    : 0
                ).Sum();

                return new
                {
                    d.ItemId,
                    d.ItemName,
                    d.ItemNameOnline,
                    d.ItemCode,
                    d.ContainerCharges,
                    d.Weight,
                    d.Unit,
                    d.Dietary,
                    d.Mrpprice,
                    d.SalePrice,
                    d.PurchasePrice,
                    d.OpeningStock,
                    d.OpeningStockPrice,
                    d.OpeningStockDate,
                    d.MinStockToMaintain,
                    d.Category,
                    d.Gst,
                    FCRCost = recipeDetails
                };
            }).ToList();

            return Ok(result);
        }




        [HttpGet("getItemData")]
        public async Task<IActionResult> GetItemData()
        {
            var data = await (
                from item in context.Items
                join category in context.ItemCategories on item.CategoryId equals category.CategoryId into categoryJoin
                from category in categoryJoin.DefaultIfEmpty()

                join gst in context.Gsts on item.GstId equals gst.GstId into gstJoin
                from gst in gstJoin.DefaultIfEmpty()

                join recipe in context.Recipes on item.ItemId equals recipe.ItemId into recipeJoin
                from recipe in recipeJoin.DefaultIfEmpty()

                select new
                {
                    item.ItemId,
                    item.ItemName,
                    item.ItemNameOnline,
                    item.ItemCode,
                    item.ContainerCharges,
                    item.Weight,
                    item.Unit,
                    item.Dietary,
                    item.Mrpprice,
                    item.SalePrice,
                    item.PurchasePrice,
                    item.OpeningStock,
                    item.OpeningStockPrice,
                    item.OpeningStockDate,
                    item.MinStockToMaintain,

                    Category = category == null ? null : new
                    {
                        item.CategoryId,
                        category.CategoryName
                    },
                    Gst = gst == null ? null : new
                    {
                        item.GstId,
                        gst.Gstpercentage,
                        gst.Cgst,
                        gst.Sgst,
                        gst.Igst
                    },

                    RecipeId = recipe != null ? recipe.RecipeId : (int?)null
                }
            ).OrderByDescending(x => x.ItemId).ToListAsync();

            // 👇 Client-side calculation for FCRCost
            var result = data.Select(d =>
            {
                decimal fcrCost = 0;

                // Only calculate if RecipeId exists
                if (d.RecipeId.HasValue)
                {
                    var recipeDetails = (
                        from rd in context.RecipeDetails
                        join rm in context.RawMaterials on rd.RawMaterialId equals rm.RawMaterialId
                        join pdg in
                            (from pd in context.PurchaseDetails
                             group pd by pd.RawMaterialId into g
                             select new
                             {
                                 RawMaterialId = g.Key,
                                 TotalPurchaseQuantity = g.Sum(x => x.Quantity) ?? 0,
                                 TotalRate = g.Sum(x => x.Rate) ?? 0,
                                 Unit = g.Select(x => x.Unit).FirstOrDefault()
                             }) on rd.RawMaterialId equals pdg.RawMaterialId
                        join cu in context.ComparisonUnits
                            on new { FromUnit = rm.PurchaseUnit, ToUnit = rm.ConsumptionUnit }
                            equals new { FromUnit = cu.FromUnit, ToUnit = cu.ToUnit } into cuJoin
                        from cu in cuJoin.DefaultIfEmpty()
                        where rd.RecipeId == d.RecipeId.Value
                        select new
                        {
                            rd.Quantity,
                            pdg.TotalPurchaseQuantity,
                            pdg.TotalRate,
                            EquivalentValue = cu != null ? cu.EquivalentValue : "1" // Default to 1 if null
                        }
                    ).ToList(); // Materialize the query

                    fcrCost = recipeDetails.Sum(x =>
                    {
                        decimal quantity = x.Quantity ?? 0;
                        decimal totalRate = x.TotalRate;
                        decimal totalPurchaseQuantity = x.TotalPurchaseQuantity;
                        decimal equivalentValue = 1;

                        // Safely parse EquivalentValue
                        if (!string.IsNullOrEmpty(x.EquivalentValue) && decimal.TryParse(x.EquivalentValue, out decimal parsedValue))
                        {
                            equivalentValue = parsedValue;
                        }

                        // Avoid division by zero
                        if (totalPurchaseQuantity * equivalentValue == 0)
                            return 0;

                        return Math.Round(
                            quantity * (totalRate / (totalPurchaseQuantity * equivalentValue)),
                            2
                        );
                    });
                }

                return new
                {
                    d.ItemId,
                    d.ItemName,
                    d.ItemNameOnline,
                    d.ItemCode,
                    d.ContainerCharges,
                    d.Weight,
                    d.Unit,
                    d.Dietary,
                    d.Mrpprice,
                    d.SalePrice,
                    d.PurchasePrice,
                    d.OpeningStock,
                    d.OpeningStockPrice,
                    d.OpeningStockDate,
                    d.MinStockToMaintain,
                    d.Category,
                    d.Gst,
                    FCRCost = fcrCost
                };
            }).ToList();

            return Ok(result);
        }







        [HttpGet("getItemById")]
        public IActionResult GetItemById(int ItemId)
        {
            var item = context.Items.FirstOrDefault(x => x.ItemId == ItemId);
            if (item == null) return NotFound("Item not found.");
            return Ok(item);
        }


        [HttpPut("UpdateItem/{id}")]
        public async Task<IActionResult> UpdateItem(int id, [FromBody] Item updatedItem)
        {
            var item = context.Items.FirstOrDefault(x => x.ItemId == id);
            if (item == null) return NotFound("Item not found for update.");

            item.ItemName = updatedItem.ItemName;
            item.ItemNameOnline = updatedItem.ItemNameOnline;
            item.CategoryId = updatedItem.CategoryId;
            item.ItemCode = updatedItem.ItemCode;
            item.ContainerCharges = updatedItem.ContainerCharges;
            item.Weight = updatedItem.Weight;
            item.Unit = updatedItem.Unit;
            item.Dietary = updatedItem.Dietary;
            item.Mrpprice = updatedItem.Mrpprice;
            item.SalePrice = updatedItem.SalePrice;
            item.PurchasePrice = updatedItem.PurchasePrice;
            item.GstId = updatedItem.GstId;
            item.OpeningStock = updatedItem.OpeningStock;
            item.OpeningStockPrice = updatedItem.OpeningStockPrice;
            item.OpeningStockDate = updatedItem.OpeningStockDate;
            item.MinStockToMaintain = updatedItem.MinStockToMaintain;


            context.SaveChanges();
            return Ok("Item Updated Successfully!");
        }



        [HttpDelete("DeleteItem/{id}")]
        public IActionResult DeleteItem(int id)
        {
            var items = context.Items.FirstOrDefault(x => x.ItemId == id);
            if (items == null)
                return NotFound("Item not found for delete.");

            context.Items.Remove(items);
            context.SaveChanges();

            return Ok("Item Deleted Successfully!");
        }


        [HttpPost("AddRawCategory")]
        public IActionResult AddRawCategory([FromBody] RawMaterialCategory request, string Action)
        {
            if (Action == "submit")
            {
                // Check if a Category with the same name already exists (case-insensitive)
                bool exists = context.RawMaterialCategories
                    .Any(l => l.CategoryName.ToLower() == request.CategoryName.ToLower());

                if (exists)
                {
                    return BadRequest("Category with the same name already exists.");
                }

                var raw = new RawMaterialCategory
                {
                    CategoryName = request.CategoryName
                };

                context.RawMaterialCategories.Add(raw);
                context.SaveChanges();

                return Ok("Category Inserted Successfully!");
            }
            else if (Action == "update")
            {
                var raw = context.RawMaterialCategories.FirstOrDefault(x => x.CategoryId == request.CategoryId);
                if (raw == null) return NotFound("Category not found for update.");

                raw.CategoryName = request.CategoryName;

                context.SaveChanges();
                return Ok("Category Updated Successfully!");
            }
            else if (Action == "delete")
            {
                var raw = context.RawMaterialCategories.FirstOrDefault(x => x.CategoryId == request.CategoryId);
                if (raw == null) return NotFound("Category not found for delete."); 

                if (IsIdInUse(context, "CategoryId", raw.CategoryId, typeof(RawMaterialCategory)))
                {
                    return Conflict("Cannot delete this Category as it is referenced in other records.");
                }

                context.RawMaterialCategories.Remove(raw);
                context.SaveChanges();
                return Ok("Category Deleted Successfully!");
            }
            return Ok();
        }



        [HttpGet("getRawCategoryData")]
        public async Task<IActionResult> GetRawCategoryData()
        {
            var data = await context.RawMaterialCategories
                .Select(c => new
                {
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName
                })
                .ToListAsync();

            return Ok(data);
        }



        [HttpPost("RawMaterial")]
        public IActionResult RawMaterial([FromBody] RawMaterial request,string Action)
        {
            if(Action == "submit")
            {
            
                request.RawMaterialId = context.RawMaterials.Any() ? context.RawMaterials.Max(x => x.RawMaterialId) + 1 : 1;


                // Check if a Item with the same name already exists (case-insensitive)
                bool exists = context.RawMaterials
                    .Any(l => l.Name.ToLower() == request.Name.ToLower());

                if (exists)
                {
                    return BadRequest("RawMaterial with the same name already exists.");
                }



                var rawmaterial = new RawMaterial
                {
                    Name = request.Name,
                    PurchaseUnit = request.PurchaseUnit,
                    ConsumptionUnit = request.ConsumptionUnit,
                    EquivalentUnit = request.EquivalentUnit,
                    CategoryId = request.CategoryId,
                    PurchasePrice = request.PurchasePrice,
                    TransferPrice = request.TransferPrice,
                    ReconciliationPrice = request.ReconciliationPrice,
                    OpeningStock = request.OpeningStock,
                    OpeningStockPrice = request.OpeningStockPrice,
                    OpeningStockDate = request.OpeningStockDate,
                    MinStockToMaintain = request.MinStockToMaintain,
                    YeildPercenatge=request.YeildPercenatge
                };

                context.RawMaterials.Add(rawmaterial);
                context.SaveChanges();


            }


            return Ok("RawMaterial Added Successfully!");

        }





        [HttpPut("RawMaterial/{id}")]
        public IActionResult UpdateRawMaterial(int id, [FromBody] RawMaterial request)
        {
            var raw = context.RawMaterials.FirstOrDefault(x => x.RawMaterialId == id);
            if (raw == null)
                return NotFound("RawMaterial not found for update.");

            // Optional: Check for name uniqueness (excluding current)
            bool nameExists = context.RawMaterials.Any(x => x.RawMaterialId != request.RawMaterialId && x.Name.ToLower() == request.Name.ToLower());
            if (nameExists)
                return BadRequest("RawMaterial with the same name already exists.");

            raw.Name = request.Name;
            raw.PurchaseUnit = request.PurchaseUnit;
            raw.ConsumptionUnit = request.ConsumptionUnit;
            raw.EquivalentUnit = request.EquivalentUnit;
            raw.CategoryId = request.CategoryId;
            raw.PurchasePrice = request.PurchasePrice;
            raw.TransferPrice = request.TransferPrice;
            raw.ReconciliationPrice = request.ReconciliationPrice;
            raw.OpeningStock = request.OpeningStock;
            raw.OpeningStockPrice = request.OpeningStockPrice;
            raw.OpeningStockDate = request.OpeningStockDate;
            raw.MinStockToMaintain = request.MinStockToMaintain;
            raw.YeildPercenatge = request.YeildPercenatge;

            context.SaveChanges();
            return Ok("RawMaterial Updated Successfully!");
        }





        [HttpDelete("RawMaterial/{id}")]
        public IActionResult DeleteRawMaterial(int id)
        {
            
            var raw = context.RawMaterials.FirstOrDefault(x => x.RawMaterialId == id);
            if (raw == null)
                return NotFound("RawMaterial not found for delete."); 

            if (IsIdInUse(context, "RawMaterialId", id, typeof(RawMaterial)))
            {
                return Conflict("Cannot delete this Rawmaterial as it is referenced in other records.");
            }

            context.RawMaterials.Remove(raw);
            context.SaveChanges();

            return Ok("RawMaterial Deleted Successfully!");
        }



        [HttpGet("getRawMaterialData")]
        public async Task<IActionResult> GetRawMaterialData()
        {
            var data = await (
                from raw in context.RawMaterials
                join category in context.RawMaterialCategories on raw.CategoryId equals category.CategoryId
                orderby raw.RawMaterialId descending // 👈 Add this line here
                select new 
                {
                    raw.RawMaterialId,
                    raw.Name,
                    raw.PurchaseUnit,
                    raw.ConsumptionUnit,
                    raw.EquivalentUnit,
                    raw.CategoryId,
                    raw.PurchasePrice,
                    raw.TransferPrice,
                    raw.ReconciliationPrice,
                    raw.OpeningStock,
                    raw.OpeningStockPrice,
                    raw.OpeningStockDate,
                    raw.MinStockToMaintain,
                    raw.YeildPercenatge,

                    Category = new
                    {
                        raw.CategoryId,
                        category.CategoryName
                    }, 

                  
                }
            ).ToListAsync();

            return Ok(data);
        }



        [HttpGet("getRawMaterialById")]
        public IActionResult getRawMaterialById(int Rawmaterialid)
        {
            var raw = context.RawMaterials.FirstOrDefault(x => x.RawMaterialId == Rawmaterialid);
            if (raw == null) return NotFound("RawMaterial  not found.");
            return Ok(raw);
        }




        [HttpPost("Recipe")]
        public async Task<IActionResult> Recipe([FromBody] Recipe request)
        {
            if (request == null || request.RecipeDetails == null || !request.RecipeDetails.Any())
            {
                return BadRequest("Recipe and at least one RecipeDetail is required.");
            }

            

            var recipe = new Recipe
            {
              
                ItemId = request.ItemId
            };

            context.Recipes.Add(recipe);
            await context.SaveChangesAsync(); // Save to get RecipeId for foreign key use

            foreach (var detail in request.RecipeDetails)
            {
                var recipeDetail = new RecipeDetail
                {
                    RecipeId = recipe.RecipeId, // this links to saved recipe
                    RawMaterialId = detail.RawMaterialId,
                    Quantity = detail.Quantity,
                    Unit = detail.Unit,
                    Description = detail.Description
                };

                context.RecipeDetails.Add(recipeDetail);
            }

            await context.SaveChangesAsync(); // Save all RecipeDetails

            return Ok(new { message = "Recipe created successfully", recipeId = recipe.RecipeId });
        }




        [HttpGet("GetAllRecipes1")]
        public async Task<IActionResult> GetAllRecipes1()
        {
            var data = await (
                from recipe in context.Recipes
                join item in context.Items on recipe.ItemId equals item.ItemId
                join category in context.ItemCategories on item.CategoryId equals category.CategoryId
                select new
                {
                    recipe.RecipeId,
                    item.ItemName,
                    category.CategoryName,
                    RecipeDetails = context.RecipeDetails
                        .Where(rd => rd.RecipeId == recipe.RecipeId)
                        .Select(rd => new
                        {
                            rd.RecipeDetailsId,
                            rd.RawMaterialId,
                            rd.Quantity,
                            rd.Unit,
                            rd.Description
                        }).ToList()
                }).ToListAsync();

            return Ok(data);
        }




        //[HttpGet("GetAllRecipes")]
        //public async Task<IActionResult> GetAllRecipes()
        //{
        //    var data = await (
        //        from recipe in context.Recipes
        //        join item in context.Items on recipe.ItemId equals item.ItemId
        //        join category in context.ItemCategories on item.CategoryId equals category.CategoryId
        //        select new
        //        {
        //            recipe.RecipeId,
        //            item.ItemName,
        //            category.CategoryName,
        //            RecipeDetails = (
        //                from rd in context.RecipeDetails
        //                join rm in context.RawMaterials on rd.RawMaterialId equals rm.RawMaterialId
        //                join pd in context.PurchaseDetails on rd.RawMaterialId equals pd.RawMaterialId
        //                join cu in context.ComparisonUnits
        //                    on new { FromUnit = rm.PurchaseUnit, ToUnit = rm.ConsumptionUnit }
        //                    equals new { cu.FromUnit, cu.ToUnit } into cuJoin
        //                from cu in cuJoin.DefaultIfEmpty()
        //                where rd.RecipeId == recipe.RecipeId
        //                select new
        //                {
        //                    rd.RecipeDetailsId,
        //                    RawMaterialName = rm.Name,
        //                    ConsumptionUnit = rm.ConsumptionUnit,
        //                    Equivalent = cu != null
        //                        ? "1 " + cu.FromUnit + " = " + cu.EquivalentValue + " " + cu.ToUnit
        //                        : null,
        //                    rd.Quantity,
        //                    rd.Unit,
        //                    RecipeDetailUnit = rd.Quantity + " " + rd.Unit,
        //                    rd.Description,
        //                    PurchaseQuantity = pd.Quantity,
        //                    PurchaseUnit = pd.Unit,
        //                    PurchaseDetailUnit = pd.Quantity + " " + pd.Unit,
        //                    pd.Rate,

        //                    // ✅ FCR Cost Calculation
        //                    Singlecost = cu != null && pd.Quantity != null && pd.Rate != null
        //                        ? Math.Round(
        //                            (decimal)(rd.Quantity ?? 0) *
        //                            ((decimal)pd.Rate / ((decimal)pd.Quantity * decimal.Parse(cu.EquivalentValue))),
        //                            2
        //                          )
        //                        : 0
        //                }).ToList()
        //        }).ToListAsync();

        //    return Ok(data);
        //}




        //----------------------Important-------------

        //[HttpGet("GetAllRecipes")]
        //public async Task<IActionResult> GetAllRecipes()
        //{
        //    var data = await (
        //        from recipe in context.Recipes
        //        join item in context.Items on recipe.ItemId equals item.ItemId
        //        join category in context.ItemCategories on item.CategoryId equals category.CategoryId
        //        select new
        //        {
        //            recipe.RecipeId,
        //            item.ItemName,
        //            category.CategoryName,

        //            // First compute RecipeDetails
        //            RecipeDetails = (
        //                from rd in context.RecipeDetails
        //                join rm in context.RawMaterials on rd.RawMaterialId equals rm.RawMaterialId
        //                join pd in context.PurchaseDetails on rd.RawMaterialId equals pd.RawMaterialId
        //                join cu in context.ComparisonUnits
        //                    on new { FromUnit = rm.PurchaseUnit, ToUnit = rm.ConsumptionUnit }
        //                    equals new { cu.FromUnit, cu.ToUnit } into cuJoin
        //                from cu in cuJoin.DefaultIfEmpty()
        //                where rd.RecipeId == recipe.RecipeId
        //                select new
        //                {
        //                    rd.RecipeDetailsId,
        //                    RawMaterialName = rm.Name,
        //                    ConsumptionUnit = rm.ConsumptionUnit,
        //                    Equivalent = cu != null
        //                        ? "1 " + cu.FromUnit + " = " + cu.EquivalentValue + " " + cu.ToUnit
        //                        : null,
        //                    rd.Quantity,
        //                    rd.Unit,
        //                    RecipeDetailUnit = rd.Quantity + " " + rd.Unit,
        //                    rd.Description,
        //                    PurchaseQuantity = pd.Quantity,
        //                    PurchaseUnit = pd.Unit,
        //                    PurchaseDetailUnit = pd.Quantity + " " + pd.Unit,
        //                    pd.Rate,

        //                    // Single item cost (FCR cost for each raw material)
        //                    Singlecost = cu != null && pd.Quantity != null && pd.Rate != null
        //                        ? Math.Round(
        //                            (decimal)(rd.Quantity ?? 0) *
        //                            ((decimal)pd.Rate / ((decimal)pd.Quantity * decimal.Parse(cu.EquivalentValue))),
        //                            2
        //                          )
        //                        : 0
        //                }).ToList()

        //        }) // Calculate FCRCost after RecipeDetails is evaluated
        //        .ToListAsync();

        //    // Now compute FCRCost (sum of Singlecost) for each recipe
        //    var result = data.Select(r => new
        //    {
        //        r.RecipeId,
        //        r.ItemName,
        //        r.CategoryName,
        //        r.RecipeDetails,
        //        FCRCost = r.RecipeDetails.Sum(d => d.Singlecost)
        //    }).ToList();

        //    return Ok(result);
        //}



      //  ------------------------IMportant Second________


        //[HttpGet("GetAllRecipes")]
        //public async Task<IActionResult> GetAllRecipes()
        //{
        //    var data = await (
        //        from recipe in context.Recipes
        //        join item in context.Items on recipe.ItemId equals item.ItemId
        //        join category in context.ItemCategories on item.CategoryId equals category.CategoryId
        //        select new
        //        {
        //            recipe.RecipeId,
        //            item.ItemName,
        //            category.CategoryName,

        //            RecipeDetails = (
        //                from rd in context.RecipeDetails
        //                join rm in context.RawMaterials on rd.RawMaterialId equals rm.RawMaterialId

        //                // Join with grouped purchase details
        //                join pdg in
        //                    (from pd in context.PurchaseDetails
        //                     group pd by pd.RawMaterialId into g
        //                     select new
        //                     {
        //                         RawMaterialId = g.Key,
        //                         TotalPurchaseQuantity = g.Sum(x => x.Quantity), 
        //                         TotalRate=g.Sum(x=>x.Rate),
        //                         AvgRate = g.Average(x => x.Rate),
        //                         Unit = g.Select(x => x.Unit).FirstOrDefault()
        //                     }) on rd.RawMaterialId equals pdg.RawMaterialId

        //                join cu in context.ComparisonUnits
        //                    on new { FromUnit = rm.PurchaseUnit, ToUnit = rm.ConsumptionUnit }
        //                    equals new { cu.FromUnit, cu.ToUnit } into cuJoin
        //                from cu in cuJoin.DefaultIfEmpty()

        //                where rd.RecipeId == recipe.RecipeId
        //                select new
        //                {
        //                    rd.RecipeDetailsId,
        //                    RawMaterialName = rm.Name,
        //                    ConsumptionUnit = rm.ConsumptionUnit,
        //                    Equivalent = cu != null
        //                        ? "1 " + cu.FromUnit + " = " + cu.EquivalentValue + " " + cu.ToUnit
        //                        : null,
        //                    rd.Quantity,
        //                    rd.Unit,
        //                    RecipeDetailUnit = rd.Quantity + " " + rd.Unit,
        //                    rd.Description,
        //                    PurchaseQuantity = pdg.TotalPurchaseQuantity,
        //                    PurchaseUnit = pdg.Unit,
        //                    PurchaseDetailUnit = pdg.TotalPurchaseQuantity + " " + pdg.Unit,
        //                    Rate = pdg.TotalRate,

        //                    // Singlecost calculation
        //                    Singlecost = cu != null && pdg.TotalPurchaseQuantity != null && pdg.AvgRate != null
        //                        ? Math.Round(
        //                            (decimal)(rd.Quantity ?? 0) *
        //                            ((decimal)pdg.TotalRate / ((decimal)pdg.TotalPurchaseQuantity * decimal.Parse(cu.EquivalentValue))),
        //                            2
        //                          )
        //                        : 0
        //                }).ToList()

        //        }).ToListAsync();

        //    var result = data.Select(r => new
        //    {
        //        r.RecipeId,
        //        r.ItemName,
        //        r.CategoryName,
        //        r.RecipeDetails,
        //        FCRCost = r.RecipeDetails.Sum(d => d.Singlecost)
        //    }).ToList();

        //    return Ok(result);
        //}










        [HttpGet("GetAllRecipes")]
        public async Task<IActionResult> GetAllRecipes()
        {
            // Step 1: Get top 15 raw material IDs from PurchaseDetails by TotalRate
            var topRawMaterialIds = await context.PurchaseDetails
                .GroupBy(pd => pd.RawMaterialId)
                .Select(g => new
                {
                    RawMaterialId = g.Key,
                    TotalRate = g.Sum(x => x.Rate)
                })
                .OrderByDescending(g => g.TotalRate)
                .Take(15)
                .Select(g => g.RawMaterialId)
                .ToListAsync();

            // Step 2: Use those top 15 IDs in the main query
            var data = await (
                from recipe in context.Recipes
                join item in context.Items on recipe.ItemId equals item.ItemId
                join category in context.ItemCategories on item.CategoryId equals category.CategoryId
                select new
                {
                    recipe.RecipeId,
                    item.ItemName,
                    category.CategoryName,

                    RecipeDetails = (
                        from rd in context.RecipeDetails
                        join rm in context.RawMaterials on rd.RawMaterialId equals rm.RawMaterialId

                        join pdg in
                            (from pd in context.PurchaseDetails
                             group pd by pd.RawMaterialId into g
                             select new
                             {
                                 RawMaterialId = g.Key,
                                 TotalPurchaseQuantity = g.Sum(x => x.Quantity),
                                 TotalRate = g.Sum(x => x.Rate),
                                 AvgRate = g.Average(x => x.Rate),
                                 Unit = g.Select(x => x.Unit).FirstOrDefault()
                             }) on rd.RawMaterialId equals pdg.RawMaterialId

                        join cu in context.ComparisonUnits
                            on new { FromUnit = rm.PurchaseUnit, ToUnit = rm.ConsumptionUnit }
                            equals new { cu.FromUnit, cu.ToUnit } into cuJoin
                        from cu in cuJoin.DefaultIfEmpty()

                        where rd.RecipeId == recipe.RecipeId
                              && topRawMaterialIds.Contains(rd.RawMaterialId) // 👈 Filter by top 15 RawMaterialIds
                        select new
                        {
                            rd.RecipeDetailsId,
                            RawMaterialName = rm.Name,
                            ConsumptionUnit = rm.ConsumptionUnit,
                            Equivalent = cu != null
                                ? "1 " + cu.FromUnit + " = " + cu.EquivalentValue + " " + cu.ToUnit
                                : null,
                            rd.Quantity,
                            rd.Unit,
                            RecipeDetailUnit = rd.Quantity + " " + rd.Unit,
                            rd.Description,
                            PurchaseQuantity = pdg.TotalPurchaseQuantity,
                            PurchaseUnit = pdg.Unit,
                            PurchaseDetailUnit = pdg.TotalPurchaseQuantity + " " + pdg.Unit,
                            Rate = pdg.TotalRate,

                            // Singlecost calculation
                            Singlecost = cu != null && pdg.TotalPurchaseQuantity != null && pdg.AvgRate != null
                                ? Math.Round(
                                    (decimal)(rd.Quantity ?? 0) *
                                    ((decimal)pdg.TotalRate / ((decimal)pdg.TotalPurchaseQuantity * decimal.Parse(cu.EquivalentValue))),
                                    2
                                  )
                                : 0
                        }).ToList()
                }).ToListAsync();

            var result = data.Select(r => new
            {
                r.RecipeId,
                r.ItemName,
                r.CategoryName,
                r.RecipeDetails,
                FCRCost = r.RecipeDetails.Sum(d => d.Singlecost)
            }).OrderByDescending(r => r.RecipeId).ToList();

            return Ok(result);
        }








        [HttpGet("GetAllRecipes2")]
        public async Task<IActionResult> GetAllRecipes2()
        {
            // Step 1: Get top 15 RawMaterialIds by total Rate from PurchaseDetails
            var topRawMaterialIds = await context.PurchaseDetails
                .GroupBy(pd => pd.RawMaterialId)
                .Select(g => new
                {
                    RawMaterialId = g.Key,
                    TotalRate = g.Sum(x => x.Rate)
                })
                .OrderByDescending(g => g.TotalRate)
                .Take(15)
                .Select(g => g.RawMaterialId)
                .ToListAsync();

            // Step 2: Get all recipes with item and category
            var recipes = await (
                from recipe in context.Recipes
                join item in context.Items on recipe.ItemId equals item.ItemId
                join category in context.ItemCategories on item.CategoryId equals category.CategoryId
                select new
                {
                    recipe.RecipeId,
                    item.ItemName,
                    category.CategoryName
                }).ToListAsync();

            // Prepare final result list
            var result = new List<object>();

            foreach (var recipe in recipes)
            {
                // Get related RecipeDetails
                var details = await (
                    from rd in context.RecipeDetails
                    join rm in context.RawMaterials on rd.RawMaterialId equals rm.RawMaterialId
                    join pdg in
                        (from pd in context.PurchaseDetails
                         group pd by pd.RawMaterialId into g
                         select new
                         {
                             RawMaterialId = g.Key,
                             TotalPurchaseQuantity = g.Sum(x => x.Quantity),
                             TotalRate = g.Sum(x => x.Rate),
                             AvgRate = g.Average(x => x.Rate),
                             Unit = g.Select(x => x.Unit).FirstOrDefault()
                         }) on rd.RawMaterialId equals pdg.RawMaterialId
                    join cu in context.ComparisonUnits
                        on new { FromUnit = rm.PurchaseUnit, ToUnit = rm.ConsumptionUnit }
                        equals new { cu.FromUnit, cu.ToUnit } into cuJoin
                    from cu in cuJoin.DefaultIfEmpty()
                    where rd.RecipeId == recipe.RecipeId
                          && topRawMaterialIds.Contains(rd.RawMaterialId)
                    select new
                    {
                        rd.RecipeDetailsId,
                        RawMaterialName = rm.Name,
                        ConsumptionUnit = rm.ConsumptionUnit,
                        Equivalent = cu != null
                            ? "1 " + cu.FromUnit + " = " + cu.EquivalentValue + " " + cu.ToUnit
                            : null,
                        rd.Quantity,
                        rd.Unit,
                        RecipeDetailUnit = rd.Quantity + " " + rd.Unit,
                        rd.Description,
                        PurchaseQuantity = pdg.TotalPurchaseQuantity,
                        PurchaseUnit = pdg.Unit,
                        PurchaseDetailUnit = pdg.TotalPurchaseQuantity + " " + pdg.Unit,
                        Rate = pdg.TotalRate,
                        Singlecost = cu != null && pdg.TotalPurchaseQuantity != null && pdg.AvgRate != null
                            ? Math.Round(
                                (decimal)(rd.Quantity ?? 0) *
                                ((decimal)pdg.TotalRate / ((decimal)pdg.TotalPurchaseQuantity * decimal.Parse(cu.EquivalentValue))),
                                2
                              )
                            : 0
                    }).ToListAsync();

                // Sum of Singlecosts (FCRCost)
                decimal fcrCost = 0;
                foreach (var d in details)
                {
                    fcrCost += d.Singlecost;
                }

                // Add result
                result.Add(new
                {
                    recipe.RecipeId,
                    recipe.ItemName,
                    recipe.CategoryName,
                    RecipeDetails = details,
                    FCRCost = Math.Round(fcrCost, 2)
                });
            }

            return Ok(result);
        }






        [HttpPut("UpdateRecipe/{id}")]
        public async Task<IActionResult> UpdateRecipe(int id, [FromBody] Recipe updatedRecipe)
        {
            if (id != updatedRecipe.RecipeId)
                return BadRequest("Recipe ID mismatch");

            if (updatedRecipe == null || updatedRecipe.RecipeDetails == null || !updatedRecipe.RecipeDetails.Any())
                return BadRequest("Recipe and at least one RecipeDetail is required.");

            // Fetch the existing recipe with details
            var existingRecipe = await context.Recipes
                .Include(r => r.RecipeDetails)
                .FirstOrDefaultAsync(r => r.RecipeId == id);

            if (existingRecipe == null)
                return NotFound("Recipe not found.");

            // Update Recipe main fields
            existingRecipe.ItemId = updatedRecipe.ItemId;

            // Sync RecipeDetails
            var existingDetails = existingRecipe.RecipeDetails.ToList();
            var incomingDetails = updatedRecipe.RecipeDetails.ToList();

            // Remove deleted details
            foreach (var existing in existingDetails)
            {
                if (!incomingDetails.Any(d => d.RecipeDetailsId == existing.RecipeDetailsId))
                {
                    context.RecipeDetails.Remove(existing);
                }
            }

            // Add or update details
            foreach (var incoming in incomingDetails)
            {
                var existing = existingDetails.FirstOrDefault(d => d.RecipeDetailsId == incoming.RecipeDetailsId);

                if (existing != null)
                {
                    // Update existing
                    existing.RawMaterialId = incoming.RawMaterialId;
                    existing.Quantity = incoming.Quantity;
                    existing.Unit = incoming.Unit;
                    existing.Description = incoming.Description;
                }
                else
                {
                    // Add new
                    var newDetail = new RecipeDetail
                    {
                        RecipeId = id,
                        RawMaterialId = incoming.RawMaterialId,
                        Quantity = incoming.Quantity,
                        Unit = incoming.Unit,
                        Description = incoming.Description
                    };
                    context.RecipeDetails.Add(newDetail);
                }
            }

            await context.SaveChangesAsync();

            return Ok(new { message = "Recipe updated successfully", recipeId = existingRecipe.RecipeId });
        }




        [HttpDelete("DeleteRecipe/{id}")]
        public async Task<IActionResult> DeleteRecipe(int id)
        {
            var recipe = await context.Recipes
                .Include(r => r.RecipeDetails) // Include related RecipeDetails
                .FirstOrDefaultAsync(r => r.RecipeId == id);

            if (recipe == null)
                return NotFound("Recipe not found.");


            //// Check if this ID is used in any other table except ComparisonUnits
            //if (IsIdInUse(context, "RecipeId", id, typeof(Recipe)))
            //{
            //    return Conflict("Cannot delete this Recipe Info as it is referenced in other records.");
            //}




            // Delete all related RecipeDetails first
            context.RecipeDetails.RemoveRange(recipe.RecipeDetails);

            // Then delete the Recipe
            context.Recipes.Remove(recipe);

            await context.SaveChangesAsync();

            return Ok(new { message = "Recipe deleted successfully", recipeId = id });
        }




        // for single record 
        [HttpGet("GetRecipeById/{id}")]
        public async Task<IActionResult> GetRecipeById(int id)
        {
            var data = await (
                from recipe in context.Recipes
                join item in context.Items on recipe.ItemId equals item.ItemId
                join category in context.ItemCategories on item.CategoryId equals category.CategoryId
                where recipe.RecipeId == id
                select new
                {
                    recipe.RecipeId,
                    recipe.ItemId,
                    item.ItemName,
                    category.CategoryName,
                    RecipeDetails = context.RecipeDetails
                        .Where(rd => rd.RecipeId == recipe.RecipeId)
                        .Select(rd => new
                        {
                            rd.RecipeDetailsId,
                            rd.RawMaterialId,
                            rd.Quantity,
                            rd.Unit,
                            rd.Description
                        }).ToList()
                }).FirstOrDefaultAsync();

            if (data == null)
                return NotFound("Recipe not found.");

            return Ok(data);
        }




        //[HttpGet("getItemDataRecipedrop")]
        //public async Task<IActionResult> getItemDataRecipedrop()
        //{
        //    // Get all ItemIds already used in Recipe table
        //    var usedItemIds = context.Recipes
        //        .Where(r => r.ItemId != null)
        //        .Select(r => r.ItemId)
        //        .Distinct();

        //    var data = await (
        //        from item in context.Items
        //        where !usedItemIds.Contains(item.ItemId) // ⛔ Exclude items already in Recipe

        //        join category in context.ItemCategories on item.CategoryId equals category.CategoryId into categoryJoin
        //        from category in categoryJoin.DefaultIfEmpty() // Left join for Category

        //        join gst in context.Gsts on item.GstId equals gst.GstId into gstJoin
        //        from gst in gstJoin.DefaultIfEmpty() // Left join for GST

        //        orderby item.ItemId descending
        //        select new
        //        {
        //            item.ItemId,
        //            item.ItemName,
        //            item.ItemNameOnline,
        //            item.ItemCode,
        //            item.ContainerCharges,
        //            item.Weight,
        //            item.Unit,
        //            item.Dietary,
        //            item.Mrpprice,
        //            item.SalePrice,
        //            item.PurchasePrice,
        //            item.OpeningStock,
        //            item.OpeningStockPrice,
        //            item.OpeningStockDate,
        //            item.MinStockToMaintain,

        //            Category = category == null ? null : new
        //            {
        //                item.CategoryId,
        //                category.CategoryName
        //            },
        //            Gst = gst == null ? null : new
        //            {
        //                item.GstId,
        //                gst.Gstpercentage,
        //                gst.Cgst,
        //                gst.Sgst,
        //                gst.Igst
        //            }
        //        }
        //    ).ToListAsync();

        //    return Ok(data);
        //}



        [HttpGet("getItemDataRecipedropedit/{itemId?}")]
        public async Task<IActionResult> getItemDataRecipedropedit(int? itemId)
        {
            // Get all ItemIds already used in Recipe table
            var usedItemIds = context.Recipes
                .Where(r => r.ItemId != null)
                .Select(r => r.ItemId)
                .Distinct();

            var data = await (
                from item in context.Items
                where !usedItemIds.Contains(item.ItemId) || item.ItemId == itemId // 👈 Allow current edited item

                join category in context.ItemCategories on item.CategoryId equals category.CategoryId into categoryJoin
                from category in categoryJoin.DefaultIfEmpty()

                join gst in context.Gsts on item.GstId equals gst.GstId into gstJoin
                from gst in gstJoin.DefaultIfEmpty()

                orderby item.ItemId descending
                select new
                {
                    item.ItemId,
                    item.ItemName,
                    item.ItemNameOnline,
                    item.ItemCode,
                    item.ContainerCharges,
                    item.Weight,
                    item.Unit,
                    item.Dietary,
                    item.Mrpprice,
                    item.SalePrice,
                    item.PurchasePrice,
                    item.OpeningStock,
                    item.OpeningStockPrice,
                    item.OpeningStockDate,
                    item.MinStockToMaintain,

                    Category = category == null ? null : new
                    {
                        item.CategoryId,
                        category.CategoryName
                    },
                    Gst = gst == null ? null : new
                    {
                        item.GstId,
                        gst.Gstpercentage,
                        gst.Cgst,
                        gst.Sgst,
                        gst.Igst
                    }
                }
            ).ToListAsync();

            return Ok(data);
        }




        [HttpGet("GetLatestPurchaseInvoiceNo")]
        public async Task<IActionResult> GetLatestPurchaseInvoiceNo()
        {
            string newInvoiceNo = "Inv-1";
            var lastSale = await context.Purchases
                .OrderByDescending(s => s.PurchaseId)
                .FirstOrDefaultAsync();

            if (lastSale != null && !string.IsNullOrEmpty(lastSale.InvoiceNo) && lastSale.InvoiceNo.StartsWith("Inv-"))
            {
                var parts = lastSale.InvoiceNo.Split('-');
                if (parts.Length == 2 && int.TryParse(parts[1], out int lastNumber))
                {
                    newInvoiceNo = $"Inv-{lastNumber + 1}";
                }
            }

            return Ok(new { InvoiceNo = newInvoiceNo });
        } 



        [HttpGet("GetCreatePurchase/{ledgerId}")]
        public async Task<IActionResult> GetCreatePurchase(int ledgerId) // 👈 removed [FromQuery]
        {


            var ledger = await context.Ledgers
               .FirstOrDefaultAsync(l => l.LedgerId == ledgerId);

            decimal LedgerAmount = ledger?.OpeningBalance ?? 0;



            decimal totalSaleBalance = await context.TransactionData
               .Where(r => r.LedgerId == ledgerId && r.TransactionMode == "Sale")
               .SumAsync(r => (decimal?)r.Balance) ?? 0;


            decimal totalPurchaseBalance = await context.TransactionData
               .Where(r => r.LedgerId == ledgerId && r.TransactionMode == "Purchase")
               .SumAsync(r => (decimal?)r.Balance) ?? 0;


            DateTime currentDate = DateTime.Now;


            var totalReceiptReceived = await context.TransactionData
                  .Where(r => r.LedgerId == ledgerId && r.Date <= currentDate && r.TransactionMode == "Receipt")
                  .SumAsync(r => (decimal?)r.Received) ?? 0;


            var totalPaymentReceived = await context.TransactionData
                  .Where(r => r.LedgerId == ledgerId && r.Date <= currentDate && r.TransactionMode == "Payment")
                  .SumAsync(r => (decimal?)r.Received) ?? 0;



            //  Final total 
            decimal totalBalance = LedgerAmount + totalSaleBalance - totalReceiptReceived - totalPurchaseBalance + totalPaymentReceived;

          

            return Ok(new
            {
                TotalSaleBalance = totalSaleBalance,
                OpeningBalance = LedgerAmount,
                TotalReceiptCredits = totalReceiptReceived,
                TotalPurchaseBalance = totalPurchaseBalance,
                TotalPurchaseCredits = totalPaymentReceived,
                TotalBalance = totalBalance
            });


        }




        [HttpPost("CreatePurchase")]
        public async Task<IActionResult> CreatePurchase([FromBody] Purchase request)
        {
            if (request == null || request.PurchaseDetails == null || !request.PurchaseDetails.Any())
            {
                return BadRequest("Purchase and at least one PurchaseDetail is required.");
            }

            // Auto-generate InvoiceNo
            string newInvoiceNo = "Inv-1";
            var lastSale = await context.Purchases
                .OrderByDescending(s => s.PurchaseId)
                .FirstOrDefaultAsync();

            if (lastSale != null && !string.IsNullOrEmpty(lastSale.InvoiceNo) && lastSale.InvoiceNo.StartsWith("Inv-"))
            {
                var parts = lastSale.InvoiceNo.Split('-');
                if (parts.Length == 2 && int.TryParse(parts[1], out int lastNumber))
                {
                    newInvoiceNo = $"Inv-{lastNumber + 1}";
                }
            }

            var purchase = new Purchase
            {
                InvoiceNo = newInvoiceNo,
                LedgerId = request.LedgerId,
                BranchId = request.BranchId,
                UserId = request.UserId,
                Date = request.Date ?? DateTime.Now,
                TotalAmount = request.TotalAmount,
                TotalGst = request.TotalGst,
                TotalDiscount = request.TotalDiscount,
                PaymentStatus = request.PaymentStatus,
                PaymentMode = request.PaymentMode,
                Balance = request.Balance,
                Received = request.Received
            };

            context.Purchases.Add(purchase);
            await context.SaveChangesAsync();

            //foreach (var detail in request.PurchaseDetails)
            //{
            //    var purchasedetail = new PurchaseDetail
            //    {

            //        PurchaseId = purchase.PurchaseId,
            //        RawMaterialId = detail.RawMaterialId,
            //        Quantity = detail.Quantity,
            //        Rate = detail.Rate,
            //        Amount = detail.Amount,
            //        DiscountPercentage = detail.DiscountPercentage,
            //        DiscountAmount = detail.DiscountAmount,
            //        Gstpercentage = detail.Gstpercentage,
            //        Gstamount = detail.Gstamount,
            //        Total = detail.Total
            //    };

            //    context.PurchaseDetails.Add(purchasedetail);
            //}

            foreach (var detail in request.PurchaseDetails)
            {
                var purchasedetail = new PurchaseDetail
                {
                    PurchaseId = purchase.PurchaseId, // ✅ correctly assigned
                    RawMaterialId = detail.RawMaterialId,
                    Unit = detail.Unit,
                    Quantity = detail.Quantity,                   
                    Rate = detail.Rate,
                    Amount = detail.Amount,
                    DiscountPercentage = detail.DiscountPercentage,
                    DiscountAmount = detail.DiscountAmount,
                    Gstpercentage = detail.Gstpercentage,
                    Gstamount = detail.Gstamount,
                    Total = detail.Total
                };

                context.PurchaseDetails.Add(purchasedetail);
            }




            await context.SaveChangesAsync();
           

            var totalLedgerBalance = await context.Purchases
                .Where(s => s.LedgerId == request.LedgerId)
                .SumAsync(s => s.Balance ?? 0);


            var ledger = await context.Ledgers
                .Where(l => l.LedgerId == request.LedgerId)
                .FirstOrDefaultAsync();

            decimal openingBalance = ledger?.OpeningBalance ?? 0;


            var totalbalance = totalLedgerBalance;


            var transpaurchase = new TransactionDatum
            {
                VoucherNo = Convert.ToString(purchase.PurchaseId),
                Date = request.Date ?? DateTime.Now,
                PayMode = request.PaymentMode,
                LedgerId = request.LedgerId,
                Total = request.TotalAmount,
                Balance = request.Balance,
                Received = request.Received,
                TransactionMode = "Purchase",
            };

            context.TransactionData.Add(transpaurchase);
            await context.SaveChangesAsync();




            return Ok(new
            {
                Message = "Purchase created successfully",
                PurchaseId = purchase.PurchaseId,
                InvoiceNo = newInvoiceNo,
                TotalLedgerBalance = totalLedgerBalance,
                TotalBalance = totalbalance,
               

            });
        }




















        [HttpGet("GetAllPurchaseData")]
        public async Task<ActionResult<IEnumerable<Purchase>>> GetAllPurchaseData()
        {
            var purchase = await context.Purchases
                .Include(s => s.Ledger)
                .OrderByDescending(purchase => purchase.PurchaseId) // 👈 Order by descending here
                .Select(purchase => new
                {
                    PurchaseId = purchase.PurchaseId,
                    InvoiceNo = purchase.InvoiceNo,
                    Date = purchase.Date,
                    TotalAmount = purchase.TotalAmount,
                    BranchId = purchase.BranchId,
                    LedgerId = purchase.LedgerId,
                    LedgerName = purchase.Ledger != null ? purchase.Ledger.LedgerName : null,
                    UserId = purchase.UserId,
                    TotalGst = purchase.TotalGst,
                    TotalDiscount = purchase.TotalDiscount,
                    PaymentStatus = purchase.PaymentStatus,
                    PaymentMode = purchase.PaymentMode,
                    Received = purchase.Received,
                    Balance = purchase.Balance
                })
                .ToListAsync();

            return Ok(purchase);
        }




        [HttpGet("GetPurchaseDataById/{purchaseId}")]
        public async Task<IActionResult> GetPurchaseDataById(int purchaseId)
        {
            var PurchaseData = await context.Purchases
                .Where(s => s.PurchaseId == purchaseId)
                .Select(s => new
                {
                    s.PurchaseId,
                    s.InvoiceNo,
                    s.Date,
                    s.TotalAmount,
                    s.BranchId,
                    s.LedgerId,
                    LedgerName = s.Ledger != null ? s.Ledger.LedgerName : null,
                    BillingAddress = s.Ledger != null ? s.Ledger.BillingAddress : null,
                    ShippingAddress = s.Ledger != null ? s.Ledger.ShipingAddress : null,
                    Contact = s.Ledger != null ? s.Ledger.MobileNumber : null,
                    s.UserId,
                    s.TotalGst,
                    s.TotalDiscount,
                    s.PaymentStatus,
                    s.PaymentMode,
                    s.Balance,
                    s.Received,
                    PurchaseDetails = s.PurchaseDetails.Select(sd => new
                    {
                        sd.PurchaseDetailId,
                        sd.RawMaterialId,
                        sd.Quantity,
                        sd.Unit,
                        sd.Rate,
                        sd.Amount,
                        sd.DiscountPercentage,
                        sd.DiscountAmount,
                        sd.Gstpercentage,
                        sd.Gstamount,
                        sd.Total,
                        Name = sd.RawMaterial != null ? sd.RawMaterial.Name : null,
                        CategoryId = sd.RawMaterial != null ? sd.RawMaterial.CategoryId : null,
                        CategoryName = sd.RawMaterial != null && sd.RawMaterial.Category != null ? sd.RawMaterial.Category.CategoryName : null,
                      
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (PurchaseData == null)
            {
                return NotFound(new { message = "Purchase not found" });
            }

            if (PurchaseData == null)
                return NotFound("Purchase not found.");

          
            var response = new
            {
                PurchaseData.PurchaseId,
                PurchaseData.InvoiceNo,
                PurchaseData.Date,
                PurchaseData.TotalAmount,
                PurchaseData.BranchId,
                PurchaseData.LedgerId,
                PurchaseData.LedgerName,
                PurchaseData.BillingAddress,
                PurchaseData.ShippingAddress,
                PurchaseData.Contact,
                PurchaseData.UserId,
                PurchaseData.TotalGst,
                PurchaseData.TotalDiscount,
                PurchaseData.PaymentStatus,
                PurchaseData.PaymentMode,
                PurchaseData.Balance,
                PurchaseData.Received,    
                PurchaseData.PurchaseDetails
            };

            return Ok(response);
        }




        [HttpGet("GetCreatePurchaseUpdate/{ledgerId}")]
        public async Task<IActionResult> GetCreatePurchaseUpdate(int ledgerId) 
        {
            var ledger = await context.Ledgers
               .FirstOrDefaultAsync(l => l.LedgerId == ledgerId);

            decimal LedgerAmount = ledger?.OpeningBalance ?? 0;


            decimal totalSaleBalance = await context.TransactionData
               .Where(r => r.LedgerId == ledgerId && r.TransactionMode == "Sale")
               .SumAsync(r => (decimal?)r.Balance) ?? 0;


            decimal totalPurchaseBalance = await context.TransactionData
               .Where(r => r.LedgerId == ledgerId && r.TransactionMode == "Purchase")
               .SumAsync(r => (decimal?)r.Balance) ?? 0;


            DateTime currentDate = DateTime.Now;


            var totalReceiptReceived = await context.TransactionData
                  .Where(r => r.LedgerId == ledgerId && r.Date <= currentDate && r.TransactionMode == "Receipt")
                  .SumAsync(r => (decimal?)r.Received) ?? 0;


            var totalPaymentReceived = await context.TransactionData
                  .Where(r => r.LedgerId == ledgerId && r.Date <= currentDate && r.TransactionMode == "Payment")
                  .SumAsync(r => (decimal?)r.Received) ?? 0;



            // ✅ Final total balance calculation
            decimal totalBalance = LedgerAmount + totalSaleBalance - totalReceiptReceived - totalPurchaseBalance + totalPaymentReceived;


            return Ok(new
            {
                TotalSaleBalance = totalSaleBalance,
                OpeningBalance = LedgerAmount,
                TotalReceiptCredits = totalReceiptReceived,
                TotalPurchaseBalance = totalPurchaseBalance,
                TotalPurchaseCredits = totalPaymentReceived,
                TotalBalance = totalBalance
            });

        }




        [HttpPut("UpdatePurchase/{id}")]
        public async Task<IActionResult> UpdatePurchase(int id, [FromBody] Purchase updatedPurchase)
        {
            if (id != updatedPurchase.PurchaseId)
                return BadRequest("Purchase ID mismatch");

            var existingPurchase = await context.Purchases
                .Include(s => s.PurchaseDetails)
                .FirstOrDefaultAsync(s => s.PurchaseId == id);

            if (existingPurchase == null)
                return NotFound("Purchase not found");

            // Update purchase main properties
            existingPurchase.InvoiceNo = updatedPurchase.InvoiceNo;
            existingPurchase.LedgerId = updatedPurchase.LedgerId;
            existingPurchase.BranchId = updatedPurchase.BranchId;
            existingPurchase.UserId = updatedPurchase.UserId;
            existingPurchase.Date = updatedPurchase.Date;
            existingPurchase.TotalAmount = updatedPurchase.TotalAmount;
            existingPurchase.TotalGst = updatedPurchase.TotalGst;
            existingPurchase.TotalDiscount = updatedPurchase.TotalDiscount;
            existingPurchase.PaymentStatus = updatedPurchase.PaymentStatus;
            existingPurchase.PaymentMode = updatedPurchase.PaymentMode;
            existingPurchase.Balance = updatedPurchase.Balance;
            existingPurchase.Received = updatedPurchase.Received;

            // Sync PurchaseDetails
            var existingDetails = existingPurchase.PurchaseDetails.ToList();
            var incomingDetails = updatedPurchase.PurchaseDetails.ToList();

            // Remove deleted details
            foreach (var detail in existingDetails)
            {
                if (!incomingDetails.Any(d => d.PurchaseDetailId == detail.PurchaseDetailId))
                {
                    context.PurchaseDetails.Remove(detail);
                }
            }

            // Add or update details
            foreach (var newDetail in incomingDetails)
            {
                var existingDetail = existingDetails
                    .FirstOrDefault(d => d.PurchaseDetailId == newDetail.PurchaseDetailId);

                if (existingDetail != null)
                {
                    // Update existing
                    existingDetail.RawMaterialId = newDetail.RawMaterialId;
                    existingDetail.Unit = newDetail.Unit;
                    existingDetail.Quantity = newDetail.Quantity;
                    existingDetail.Rate = newDetail.Rate;
                    existingDetail.Amount = newDetail.Amount;
                    existingDetail.DiscountPercentage = newDetail.DiscountPercentage;
                    existingDetail.DiscountAmount = newDetail.DiscountAmount;
                    existingDetail.Gstpercentage = newDetail.Gstpercentage;
                    existingDetail.Gstamount = newDetail.Gstamount;
                    existingDetail.Total = newDetail.Total;
                }
                else
                {
                    // Add new
                    var detailToAdd = new PurchaseDetail
                    {
                        PurchaseId = existingPurchase.PurchaseId, // safer
                        RawMaterialId = newDetail.RawMaterialId, 
                        Unit= newDetail.Unit,
                        Quantity = newDetail.Quantity,
                        Rate = newDetail.Rate,
                        Amount = newDetail.Amount,
                        DiscountPercentage = newDetail.DiscountPercentage,
                        DiscountAmount = newDetail.DiscountAmount,
                        Gstpercentage = newDetail.Gstpercentage,
                        Gstamount = newDetail.Gstamount,
                        Total = newDetail.Total
                    };
                    context.PurchaseDetails.Add(detailToAdd);
                }
            }

            // Update TransactionData if exists
            var transaction = await context.TransactionData
                .FirstOrDefaultAsync(t => t.TransactionMode == "Purchase" && t.VoucherNo == id.ToString());

            if (transaction != null)
            {
                transaction.Date = updatedPurchase.Date ?? DateTime.Now;
                transaction.PayMode = updatedPurchase.PaymentMode;
                transaction.LedgerId = updatedPurchase.LedgerId;
                transaction.Balance = updatedPurchase.Balance;
                transaction.Received = updatedPurchase.Received;
                transaction.Total = updatedPurchase.TotalAmount;
                transaction.TransactionMode = "Purchase"; // already known
            }

            await context.SaveChangesAsync();
            return Ok("Purchase updated successfully.");
        }



        //[HttpDelete("DeletePurchase/{id}")]
        //public IActionResult DeletePurchase(int id)
        //{
        //    var purchase = context.Purchases
        //                          .Include(p => p.PurchaseDetails)
        //                          .FirstOrDefault(x => x.PurchaseId == id);

        //    if (purchase == null)
        //        return NotFound("Purchase Data not found for delete.");

        //    // First remove all related PurchaseDetails
        //    context.PurchaseDetails.RemoveRange(purchase.PurchaseDetails);

        //    // Then remove the main Purchase record
        //    context.Purchases.Remove(purchase);

        //    context.SaveChanges();

        //    return Ok("Purchase and its details deleted successfully.");
        //}



        //[HttpDelete("DeletePurchase/{id}")]
        //public IActionResult DeletePurchase(int id)
        //{
          
        //    var purchase = context.Purchases
        //                          .Include(p => p.PurchaseDetails)
        //                          .FirstOrDefault(x => x.PurchaseId == id);

        //    if (purchase == null)
        //        return NotFound("Purchase Data not found for delete.");


        //    //// Check if this ID is used in any other table except ComparisonUnits
        //    //if (IsIdInUse(context, "PurchaseId", id, typeof(Purchase)))
        //    //{
        //    //    return Conflict("Cannot delete this Purchase Info as it is referenced in other records.");
        //    //}

        //    // First remove all related PurchaseDetails
        //    context.PurchaseDetails.RemoveRange(purchase.PurchaseDetails);

        //    // Then remove the main Purchase record
        //    context.Purchases.Remove(purchase);

        //    context.SaveChanges();

        //    return Ok("Purchase and its details deleted successfully.");
        //}




        [HttpDelete("DeletePurchase/{id}")]
        public IActionResult DeletePurchase(int id)
        {
            var purchase = context.Purchases
                .Include(p => p.PurchaseDetails)
                .FirstOrDefault(x => x.PurchaseId == id);

            if (purchase == null)
                return NotFound("Purchase Data not found for delete.");

            // 🔍 Check if this PurchaseId is used in TransactionData
            bool isReferenced = context.TransactionData.Any(t =>
                t.TransactionMode == "Payment" && t.VoucherNo == id.ToString());

            if (isReferenced)
            {
                return Conflict("Cannot delete this Purchase info as it is referenced in Transaction Data.");
            }


            var relatedTransactions = context.TransactionData
            .Where(t => t.TransactionMode == "Purchase" && t.VoucherNo == id.ToString())
            .ToList();

            // 🗑 Remove related TransactionData if exists
            if (relatedTransactions.Any())
            {
                context.TransactionData.RemoveRange(relatedTransactions);
            }



            // 🗑 Remove related PurchaseDetails first
            context.PurchaseDetails.RemoveRange(purchase.PurchaseDetails);

            // 🗑 Then remove main Purchase
            context.Purchases.Remove(purchase);

            context.SaveChanges();

            return Ok("Purchase and its details deleted successfully.");
        }



        [HttpGet("GetPurchaseWithDetailsByInvoice")]
        public async Task<IActionResult> GetPurchaseWithDetailsByInvoice(int purchaseId)
        {
            // Get the purchase header along with LedgerName
            var purchaseHeader = await context.Purchases
                .Where(s => s.PurchaseId == purchaseId)
                .Select(s => new
                {
                    s.PurchaseId,
                    s.InvoiceNo,
                    s.Date,
                    s.TotalAmount,
                    s.BranchId,
                    s.LedgerId,
                    LedgerName = s.Ledger != null ? s.Ledger.LedgerName : null,
                    s.UserId,
                    s.TotalGst,
                    s.TotalDiscount,
                    s.PaymentStatus,
                    s.PaymentMode,
                    s.Balance,
                    s.Received
                })
                .FirstOrDefaultAsync();



            if (purchaseHeader == null)
                return NotFound("Purchase not found.");

       

            // Get the purchase details with Rawmaterial
            var purchaseDetails = await (from d in context.PurchaseDetails
                                         join i in context.RawMaterials on d.RawMaterialId equals i.RawMaterialId
                                         where d.PurchaseId == purchaseId
                                         select new
                                         {
                                             d.RawMaterialId,
                                             RawMaterialName = i.Name,
                                             d.Quantity,
                                             d.Rate,
                                             d.Amount,
                                             d.DiscountAmount,
                                             d.DiscountPercentage,
                                             d.Gstpercentage,
                                             d.Gstamount,
                                             d.Total
                                         }).ToListAsync();

            // Combine into a structured response
            var response = new
            {
                purchaseHeader.InvoiceNo,
                purchaseHeader.Date,
                purchaseHeader.TotalAmount,
                purchaseHeader.BranchId,
                purchaseHeader.LedgerId,
                purchaseHeader.LedgerName,
                purchaseHeader.UserId,
                purchaseHeader.TotalGst,
                purchaseHeader.TotalDiscount,
                purchaseHeader.PaymentStatus,
                purchaseHeader.PaymentMode,
                purchaseHeader.Balance,
                purchaseHeader.Received,
                Rawmaterials = purchaseDetails
            };

            return Ok(response);
        }




        [HttpGet("GetPurchaseDataSupplierById/{id}")]
        public async Task<ActionResult<IEnumerable<Purchase>>> GetPurchaseDataSupplierById(int id)
        {
            var purchase = await context.Purchases.Where(p=>p.LedgerId == id)
                .Include(s => s.Ledger)
                .OrderByDescending(purchase => purchase.PurchaseId) // 👈 Order by descending here
                .Select(purchase => new
                {
                    PurchaseId = purchase.PurchaseId,
                    InvoiceNo = purchase.InvoiceNo,
                    Date = purchase.Date,
                    TotalAmount = purchase.TotalAmount,
                    BranchId = purchase.BranchId,
                    LedgerId = purchase.LedgerId,
                    LedgerName = purchase.Ledger != null ? purchase.Ledger.LedgerName : null,
                    UserId = purchase.UserId,
                    TotalGst = purchase.TotalGst,
                    TotalDiscount = purchase.TotalDiscount,
                    PaymentStatus = purchase.PaymentStatus,
                    PaymentMode = purchase.PaymentMode,
                    Received = purchase.Received,
                    Balance = purchase.Balance
                })
                .ToListAsync();

            return Ok(purchase);
        }




        [HttpGet("GetPaymentDataSupplierById/{id}")]
        public async Task<ActionResult<IEnumerable<object>>> GetPaymentDataSupplierById(int id)
        {
            // Step 1: Get all purchase transactions for the supplier
            var purchases = await context.TransactionData
                .Where(p => p.LedgerId == id && p.TransactionMode == "Purchase")
                .Include(p => p.Ledger)
                .OrderByDescending(p => p.TransactionId)
                .ToListAsync();

            // Step 2: Prepare result with additional calculations
            var result = purchases.Select(purchase =>
            {
                // Get the sum of Received from related Payment transactions (by VoucherNo)
                var totalReceived = context.TransactionData
                    .Where(t => t.TransactionMode == "Payment" && t.VoucherNo == purchase.VoucherNo)
                    .Sum(t => t.Received ?? 0);

                var totalReceivedAmount = purchase.Received + totalReceived;

                var totalAmount = purchase.Total ?? 0;
                var balance = totalAmount - totalReceivedAmount;

                return new
                {
                    PurchaseId = purchase.TransactionId,  //TransId 
                    VoucherNo=purchase.VoucherNo,
                    Date = purchase.Date,
                    TotalAmount = totalAmount,
                    LedgerId = purchase.LedgerId,
                    LedgerName = purchase.Ledger?.LedgerName,
                    PaymentStatus = purchase.Status,
                    PaymentMode = purchase.PayMode,
                    Received = totalReceivedAmount,
                    Balance = balance

                };
            }).ToList();

            return Ok(result);
        }


        //[HttpGet("getledgerdataforpayment")]
        //public async Task<IActionResult> getledgerdataforpayment()
        //{
        //    var purchases = await context.TransactionData
        //            .Where(p => p.LedgerId == id && p.TransactionMode == "Purchase")
        //            .Include(p => p.Ledger)
        //            .OrderByDescending(p => p.TransactionId)
        //            .ToListAsync();


        //    var data = await (
        //        from ledger in context.Ledgers
        //        join ledgertype in context.LedgerTypes on ledger.LedgerTypeId equals ledgertype.LedgerTypeId
        //        orderby ledger.LedgerId descending // 👈 Order by LedgerId descending
        //        select new
        //        {
        //            LedgerId = ledger.LedgerId,
        //            LedgerName = ledger.LedgerName,
        //            LedgerType = ledgertype != null ? ledgertype.LedgerTypeName : null,
        //            Gsttype = ledger.Gsttype,
        //            Gstnumber = ledger.Gstnumber,
        //            MobileNumber = ledger.MobileNumber,
        //            PhoneNumber = ledger.PhoneNumber,
        //            BillingAddress = ledger.BillingAddress,
        //            ShipingAddress = ledger.ShipingAddress,
        //            EmailId = ledger.EmailId,
        //            State = ledger.State,
        //            OpeningBalance = ledger.OpeningBalance,
        //            OpeningBalanceDate = ledger.OpeningBalanceDate,
        //            CreditLimitStatus = ledger.CreditLimitStatus,
        //            CreditLimit = ledger.CreditLimit,
        //            LedgerTypeId = ledger.LedgerTypeId
        //        }
        //    ).ToListAsync();

        //    return Ok(data);
        //}



        //[HttpGet("getledgerdataforpayment")]
        //public async Task<IActionResult> GetLedgerDataForPayment()
        //{
        //    var data = await (
        //        from ledger in context.Ledgers
        //        join ledgertype in context.LedgerTypes on ledger.LedgerTypeId equals ledgertype.LedgerTypeId into lt
        //        from ledgertype in lt.DefaultIfEmpty()
        //        select new
        //        {
        //            LedgerId = ledger.LedgerId,
        //            LedgerName = ledger.LedgerName,
        //            LedgerType = ledgertype != null ? ledgertype.LedgerTypeName : null,
        //            Gsttype = ledger.Gsttype,
        //            Gstnumber = ledger.Gstnumber,
        //            MobileNumber = ledger.MobileNumber,
        //            PhoneNumber = ledger.PhoneNumber,
        //            BillingAddress = ledger.BillingAddress,
        //            ShipingAddress = ledger.ShipingAddress,
        //            EmailId = ledger.EmailId,
        //            State = ledger.State,
        //            OpeningBalance = ledger.OpeningBalance,
        //            OpeningBalanceDate = ledger.OpeningBalanceDate,
        //            CreditLimitStatus = ledger.CreditLimitStatus,
        //            CreditLimit = ledger.CreditLimit,
        //            LedgerTypeId = ledger.LedgerTypeId,

        //            // Total from Purchase only
        //            TotalPurchase = (context.TransactionData
        //                .Where(t => t.LedgerId == ledger.LedgerId && t.TransactionMode == "Purchase")
        //                .Sum(t => (decimal?)t.Total) ?? 0),

        //            // Received from Purchase or Payment
        //            TotalReceived = (context.TransactionData
        //                .Where(t => t.LedgerId == ledger.LedgerId &&
        //                            (t.TransactionMode == "Purchase" || t.TransactionMode == "Payment"))
        //                .Sum(t => (decimal?)t.Received) ?? 0),

        //            // Final Balance = Total - Received
        //            TotalBalance = (
        //                (context.TransactionData
        //                    .Where(t => t.LedgerId == ledger.LedgerId && t.TransactionMode == "Purchase")
        //                    .Sum(t => (decimal?)t.Total) ?? 0)
        //                -
        //                (context.TransactionData
        //                    .Where(t => t.LedgerId == ledger.LedgerId &&
        //                                (t.TransactionMode == "Purchase" || t.TransactionMode == "Payment"))
        //                    .Sum(t => (decimal?)t.Received) ?? 0)
        //            )
        //        }
        //    ).OrderByDescending(x => x.LedgerId).ToListAsync();

        //    return Ok(data);
        //}













        [HttpGet("getledgerdataforpayment")]
        public async Task<IActionResult> GetLedgerDataForPayment()
        {
            var data = await (
                from ledger in context.Ledgers
                join ledgertype in context.LedgerTypes
                    on ledger.LedgerTypeId equals ledgertype.LedgerTypeId into lt
                from ledgertype in lt.DefaultIfEmpty()
                where ledger.LedgerTypeId == 5   // ✅ only LedgerTypeId = 5
                select new
                {
                    LedgerId = ledger.LedgerId,
                    LedgerName = ledger.LedgerName,
                    LedgerType = ledgertype != null ? ledgertype.LedgerTypeName : null,
                    Gsttype = ledger.Gsttype,
                    Gstnumber = ledger.Gstnumber,
                    MobileNumber = ledger.MobileNumber,
                    PhoneNumber = ledger.PhoneNumber,
                    BillingAddress = ledger.BillingAddress,
                    ShipingAddress = ledger.ShipingAddress,
                    EmailId = ledger.EmailId,
                    State = ledger.State,
                    OpeningBalance = ledger.OpeningBalance,
                    OpeningBalanceDate = ledger.OpeningBalanceDate,
                    CreditLimitStatus = ledger.CreditLimitStatus,
                    CreditLimit = ledger.CreditLimit,
                    LedgerTypeId = ledger.LedgerTypeId,

                    // Total from Purchase only
                    TotalPurchase = (context.TransactionData
                        .Where(t => t.LedgerId == ledger.LedgerId && t.TransactionMode == "Purchase")
                        .Sum(t => (decimal?)t.Total) ?? 0),

                    // Received from Purchase or Payment
                    TotalReceived = (context.TransactionData
                        .Where(t => t.LedgerId == ledger.LedgerId &&
                                    (t.TransactionMode == "Purchase" || t.TransactionMode == "Payment"))
                        .Sum(t => (decimal?)t.Received) ?? 0),

                    // Final Balance = Total - Received
                    TotalBalance = (
                        (context.TransactionData
                            .Where(t => t.LedgerId == ledger.LedgerId && t.TransactionMode == "Purchase")
                            .Sum(t => (decimal?)t.Total) ?? 0)
                        -
                        (context.TransactionData
                            .Where(t => t.LedgerId == ledger.LedgerId &&
                                        (t.TransactionMode == "Purchase" || t.TransactionMode == "Payment"))
                            .Sum(t => (decimal?)t.Received) ?? 0)
                    )
                }
            ).OrderByDescending(x => x.LedgerId).ToListAsync();

            return Ok(data);
        }







        [HttpPost("Payment")]
        public IActionResult Payment([FromBody] TransactionDatum request)
        {

            var payment = new TransactionDatum
            {
                Date = request.Date,
                PayMode = request.PayMode,
                LedgerId = request.LedgerId,
                Total = request.Received,
                Received = request.Received,
                Balance = 0,
               // TransactionMode = request.TransactionMode,
                TransactionMode = "Payment",
                Status = request.Status,
                VoucherNo = request.VoucherNo,
                Narration = request.Narration
            };

            context.TransactionData.Add(payment);
            context.SaveChanges();

            return Ok("Payment Added Successfully!");

        }




        [HttpGet("GetAllSaleData")]
        public async Task<ActionResult<IEnumerable<Sale>>> GetAllSaleData()
        {
            var sales = await context.Sales
                .Include(s => s.Ledger)
                .OrderByDescending(sale => sale.SaleId) // 👈 Order by descending here
                .Select(sale => new
                {
                    SaleId = sale.SaleId,
                    InvoiceNo = sale.InvoiceNo,
                    Date = sale.Date,
                    TotalAmount = sale.TotalAmount,
                    BranchId = sale.BranchId,
                    LedgerId = sale.LedgerId,
                    LedgerName = sale.Ledger != null ? sale.Ledger.LedgerName : null,
                    UserId = sale.UserId,
                    TotalGst = sale.TotalGst,
                    TotalDiscount = sale.TotalDiscount,
                    PaymentStatus = sale.PaymentStatus,
                    PaymentMode = sale.PaymentMode,
                    Received = sale.Received,
                    Balance = sale.Balance
                })
                .ToListAsync();

            return Ok(sales);
        }


        [HttpGet("GetLatestInvoiceNo")]
        public async Task<IActionResult> GetLatestInvoiceNo()
        {
            string newInvoiceNo = "Inv-1"; // Default if no existing sale
            var lastSale = await context.Sales
                .OrderByDescending(s => s.SaleId)
                .FirstOrDefaultAsync();

            if (lastSale != null && !string.IsNullOrEmpty(lastSale.InvoiceNo) && lastSale.InvoiceNo.StartsWith("Inv-"))
            {
                var parts = lastSale.InvoiceNo.Split('-');
                if (parts.Length == 2 && int.TryParse(parts[1], out int lastNumber))
                {
                    newInvoiceNo = $"Inv-{lastNumber + 1}";
                }
            }

            return Ok(new { InvoiceNo = newInvoiceNo });
        }




        [HttpGet("GetCreateSale/{ledgerId}")]
        public async Task<IActionResult> GetCreateSale(int ledgerId)
        {
            // ✅ Get the OpeningBalance from the Ledger table
            var ledger = await context.Ledgers
                .FirstOrDefaultAsync(l => l.LedgerId == ledgerId);

            decimal LedgerAmount = ledger?.OpeningBalance ?? 0;



            decimal totalSaleBalance = await context.TransactionData
               .Where(r => r.LedgerId == ledgerId && r.TransactionMode == "Sale")
               .SumAsync(r => (decimal?)r.Balance) ?? 0;


            decimal totalPurchaseBalance = await context.TransactionData
               .Where(r => r.LedgerId == ledgerId && r.TransactionMode == "Purchase")
               .SumAsync(r => (decimal?)r.Balance) ?? 0;


            DateTime currentDate = DateTime.Now;


            var totalReceiptReceived = await context.TransactionData
                  .Where(r => r.LedgerId == ledgerId && r.Date <= currentDate && r.TransactionMode == "Receipt")
                  .SumAsync(r => (decimal?)r.Received) ?? 0;


            var totalPaymentReceived = await context.TransactionData
                  .Where(r => r.LedgerId == ledgerId && r.Date <= currentDate && r.TransactionMode == "Payment")
                  .SumAsync(r => (decimal?)r.Received) ?? 0;



            // ✅ Final total balance calculation
            decimal totalBalance = LedgerAmount + totalSaleBalance - totalReceiptReceived - totalPurchaseBalance + totalPaymentReceived;

            return Ok(new
            {
                TotalSaleBalance = totalSaleBalance,
                OpeningBalance = LedgerAmount,
                TotalReceiptCredits = totalReceiptReceived,
                TotalPurchaseBalance = totalPurchaseBalance,
                TotalPurchaseCredits = totalPaymentReceived,
                TotalBalance = totalBalance
            });
        }




        //[HttpPost("CreateSale")]
        //public async Task<IActionResult> CreateSale([FromBody] Sale request)
        //{
        //    if (request == null || request.SaleDetails == null || !request.SaleDetails.Any())
        //    {
        //        return BadRequest("Sale and at least one SaleDetail is required.");
        //    }



        //    Ledger? ledger = null;
        //    if (request.LedgerId.HasValue && request.LedgerId > 0)
        //    {
        //        // Existing ledger
        //        ledger = await context.Ledgers.FirstOrDefaultAsync(l => l.LedgerId == request.LedgerId);
        //    }
        //    else
        //    {
        //        // New Ledger creation (you can take values from request or set defaults)
        //        ledger = new Ledger
        //        {
        //            LedgerName = request.Ledger?.LedgerName ?? "New Ledger",
        //            LedgerTypeId = 1005,
        //            MobileNumber = request.Ledger?.MobileNumber,
        //            BillingAddress = request.Ledger?.BillingAddress,
        //            ShipingAddress = request.Ledger?.ShipingAddress,
        //            EmailId = request.Ledger?.EmailId,
        //            Gstnumber = request.Ledger?.Gstnumber,
        //            OpeningBalance = request.Ledger?.OpeningBalance ?? 0,
        //            OpeningBalanceDate = DateTime.Now
        //        };

        //        context.Ledgers.Add(ledger);
        //        await context.SaveChangesAsync();

        //        // Assign generated LedgerId back to request
        //        request.LedgerId = ledger.LedgerId;
        //    }



        //    // Auto-generate InvoiceNo
        //    string newInvoiceNo = "Inv-1";
        //    var lastSale = await context.Sales
        //        .OrderByDescending(s => s.SaleId)
        //        .FirstOrDefaultAsync();

        //    if (lastSale != null && !string.IsNullOrEmpty(lastSale.InvoiceNo) && lastSale.InvoiceNo.StartsWith("Inv-"))
        //    {
        //        var parts = lastSale.InvoiceNo.Split('-');
        //        if (parts.Length == 2 && int.TryParse(parts[1], out int lastNumber))
        //        {
        //            newInvoiceNo = $"Inv-{lastNumber + 1}";
        //        }
        //    }

        //    var sale = new Sale
        //    {
        //        InvoiceNo = newInvoiceNo,
        //        LedgerId = request.LedgerId,
        //        BranchId = request.BranchId,
        //        UserId = request.UserId,
        //        Date = request.Date ?? DateTime.Now,
        //        TotalAmount = request.TotalAmount,
        //        TotalGst = request.TotalGst,
        //        TotalDiscount = request.TotalDiscount,
        //        PaymentStatus = request.PaymentStatus,
        //        PaymentMode = request.PaymentMode,
        //        Balance = request.Balance,
        //        Received = request.Received
        //    };

        //    context.Sales.Add(sale);
        //    await context.SaveChangesAsync();

        //    foreach (var detail in request.SaleDetails)
        //    {
        //        var saleDetail = new SaleDetail
        //        {
        //            SaleId = sale.SaleId,
        //            ItemId = detail.ItemId,
        //            Hsncode = detail.Hsncode,
        //            Quantity = detail.Quantity,
        //            Rate = detail.Rate,
        //            Amount = detail.Amount,
        //            DiscountPercentage = detail.DiscountPercentage,
        //            DiscountAmount = detail.DiscountAmount,
        //            Gstpercentage = detail.Gstpercentage,
        //            Gstamount = detail.Gstamount,
        //            Total = detail.Total
        //        };

        //        context.SaleDetails.Add(saleDetail);
        //    }

        //    await context.SaveChangesAsync();


        //    // 🔴 Calculate total balance and total sales for the LedgerId
        //    var totalLedgerBalance = await context.Sales
        //        .Where(s => s.LedgerId == request.LedgerId)
        //        .SumAsync(s => s.Balance ?? 0);

        //    //// 🔄 Get the OpeningBalance of the Ledger
        //    //var ledger1 = await context.Ledgers
        //    //    .Where(l => l.LedgerId == request.LedgerId)
        //    //    .FirstOrDefaultAsync();

        //    decimal openingBalance = ledger?.OpeningBalance ?? 0;

        //    // ➕ Include OpeningBalance in total balance
        //    var totalbalance = totalLedgerBalance + openingBalance;






        //    var transsale = new TransactionDatum
        //    {
        //        VoucherNo = Convert.ToString(sale.SaleId),
        //        Date = request.Date ?? DateTime.Now,
        //        PayMode = request.PaymentMode,
        //        LedgerId = request.LedgerId,
        //        Total = request.TotalAmount,
        //        Balance = request.Balance,
        //        Received = request.Received,
        //        TransactionMode = "Sale",

        //    };

        //    context.TransactionData.Add(transsale);
        //    await context.SaveChangesAsync();

        //    return Ok(new
        //    {
        //        Message = "Sale created successfully",
        //        SaleId = sale.SaleId,
        //        InvoiceNo = newInvoiceNo,
        //        TotalLedgerBalance = totalLedgerBalance,
        //        TotalBalance = totalbalance,
        //        OpeningBalance = openingBalance,

        //    });
        //}





        //[HttpPost("CreateSale")]
        //public async Task<IActionResult> CreateSale([FromBody] Sale request)
        //{
        //    if (request == null || request.SaleDetails == null || !request.SaleDetails.Any())
        //    {
        //        return BadRequest("Sale and at least one SaleDetail is required.");
        //    }

        //    Ledger? ledger = null;

        //    // ✅ Check if existing ledger is provided
        //    if (request.LedgerId.HasValue && request.LedgerId > 0)
        //    {
        //        ledger = await context.Ledgers.FirstOrDefaultAsync(l => l.LedgerId == request.LedgerId);
        //    }
        //    else
        //    {
        //        // ✅ Create new ledger
        //        ledger = new Ledger
        //        {
        //            LedgerName = request.Ledger?.LedgerName ?? "New Ledger",
        //            LedgerTypeId = 6,
        //            MobileNumber = request.Ledger?.MobileNumber,
        //            BillingAddress = request.Ledger?.BillingAddress,
        //            ShipingAddress = request.Ledger?.ShipingAddress,
        //            EmailId = request.Ledger?.EmailId,
        //            Gstnumber = request.Ledger?.Gstnumber,
        //            OpeningBalance = request.Ledger?.OpeningBalance ?? 0,
        //            OpeningBalanceDate = DateTime.Now
        //        };

        //        context.Ledgers.Add(ledger);
        //        await context.SaveChangesAsync();

        //        // ✅ Now assign the generated LedgerId
        //        request.LedgerId = ledger.LedgerId;
        //    }



        //    //// ✅ Check if existing ledger is provided
        //    //if (request.LedgerId.HasValue && request.LedgerId > 0)
        //    //{
        //    //    ledger = await context.Ledgers
        //    //        .FirstOrDefaultAsync(l => l.LedgerId == request.LedgerId);
        //    //}
        //    //else
        //    //{
        //    //    // ✅ Always create a new ledger (even if same name already exists)
        //    //    ledger = new Ledger
        //    //    {
        //    //        LedgerName = request.Ledger?.LedgerName ?? "New Ledger",
        //    //        LedgerTypeId = 6,
        //    //        MobileNumber = request.Ledger?.MobileNumber,
        //    //        BillingAddress = request.Ledger?.BillingAddress,
        //    //        ShipingAddress = request.Ledger?.ShipingAddress,
        //    //        EmailId = request.Ledger?.EmailId,
        //    //        Gstnumber = request.Ledger?.Gstnumber,
        //    //        OpeningBalance = request.Ledger?.OpeningBalance ?? 0,
        //    //        OpeningBalanceDate = DateTime.Now
        //    //    };

        //    //    context.Ledgers.Add(ledger);
        //    //    await context.SaveChangesAsync();

        //    //    // ✅ Now assign the generated LedgerId
        //    //    request.LedgerId = ledger.LedgerId;
        //    //}





        //    // ✅ Auto-generate InvoiceNo
        //    string newInvoiceNo = "Inv-1";
        //    var lastSale = await context.Sales
        //        .OrderByDescending(s => s.SaleId)
        //        .FirstOrDefaultAsync();

        //    if (lastSale != null && !string.IsNullOrEmpty(lastSale.InvoiceNo) && lastSale.InvoiceNo.StartsWith("Inv-"))
        //    {
        //        var parts = lastSale.InvoiceNo.Split('-');
        //        if (parts.Length == 2 && int.TryParse(parts[1], out int lastNumber))
        //        {
        //            newInvoiceNo = $"Inv-{lastNumber + 1}";
        //        }
        //    }

        //    // ✅ Create Sale entry
        //    var sale = new Sale
        //    {
        //        InvoiceNo = newInvoiceNo,
        //        LedgerId = request.LedgerId,   // Always not null now
        //        BranchId = request.BranchId,
        //        UserId = request.UserId,
        //        Date = request.Date ?? DateTime.Now,
        //        TotalAmount = request.TotalAmount,
        //        TotalGst = request.TotalGst,
        //        TotalDiscount = request.TotalDiscount,
        //        PaymentStatus = request.PaymentStatus,
        //        PaymentMode = request.PaymentMode,
        //        Balance = request.Balance,
        //        Received = request.Received
        //    };

        //    context.Sales.Add(sale);
        //    await context.SaveChangesAsync();

        //    // ✅ Add SaleDetails
        //    foreach (var detail in request.SaleDetails)
        //    {
        //        var saleDetail = new SaleDetail
        //        {
        //            SaleId = sale.SaleId,
        //            ItemId = detail.ItemId,
        //            Hsncode = detail.Hsncode,
        //            Quantity = detail.Quantity,
        //            Rate = detail.Rate,
        //            Amount = detail.Amount,
        //            DiscountPercentage = detail.DiscountPercentage,
        //            DiscountAmount = detail.DiscountAmount,
        //            Gstpercentage = detail.Gstpercentage,
        //            Gstamount = detail.Gstamount,
        //            Total = detail.Total
        //        };

        //        context.SaleDetails.Add(saleDetail);
        //    }

        //    await context.SaveChangesAsync();

        //    // ✅ Calculate total ledger balance
        //    var totalLedgerBalance = await context.Sales
        //        .Where(s => s.LedgerId == request.LedgerId)
        //        .SumAsync(s => s.Balance ?? 0);

        //    decimal openingBalance = ledger?.OpeningBalance ?? 0;
        //    var totalbalance = totalLedgerBalance + openingBalance;

        //    // ✅ Save Transaction entry (always uses correct LedgerId now)
        //    var transsale = new TransactionDatum
        //    {
        //        VoucherNo = sale.SaleId.ToString(),
        //        Date = request.Date ?? DateTime.Now,
        //        PayMode = request.PaymentMode,
        //        LedgerId = request.LedgerId,   // ✅ Safe now
        //        Total = request.TotalAmount,
        //        Balance = request.Balance,
        //        Received = request.Received,
        //        TransactionMode = "Sale"
        //    };

        //    context.TransactionData.Add(transsale);
        //    await context.SaveChangesAsync();

        //    return Ok(new
        //    {
        //        Message = "Sale created successfully",
        //        SaleId = sale.SaleId,
        //        InvoiceNo = newInvoiceNo,
        //        TotalLedgerBalance = totalLedgerBalance,
        //        TotalBalance = totalbalance,
        //        OpeningBalance = openingBalance
        //    });
        //}






















        [HttpPost("CreateSale")]
        public async Task<IActionResult> CreateSale([FromBody] Sale request)
        {
            if (request == null || request.SaleDetails == null || !request.SaleDetails.Any())
            {
                return BadRequest("Sale and at least one SaleDetail is required.");
            }

            // ✅ Step 1: Create Ledger (from nested object inside Sale)
            var ledger = new Ledger
            {
                LedgerName = request.Ledger?.LedgerName,
                LedgerTypeId = 6,
                MobileNumber = request.Ledger?.MobileNumber,
                BillingAddress = request.Ledger?.BillingAddress,
                ShipingAddress = request.Ledger?.ShipingAddress,
                OpeningBalance = 0
            };

            context.Ledgers.Add(ledger);
            await context.SaveChangesAsync();

            // ✅ Step 2: Auto-generate InvoiceNo
            string newInvoiceNo = "Inv-1";
            var lastSale = await context.Sales
                .OrderByDescending(s => s.SaleId)
                .FirstOrDefaultAsync();

            if (lastSale != null && !string.IsNullOrEmpty(lastSale.InvoiceNo) && lastSale.InvoiceNo.StartsWith("Inv-"))
            {
                var parts = lastSale.InvoiceNo.Split('-');
                if (parts.Length == 2 && int.TryParse(parts[1], out int lastNumber))
                {
                    newInvoiceNo = $"Inv-{lastNumber + 1}";
                }
            }

            // ✅ Step 3: Create Sale
            var sale = new Sale
            {
                InvoiceNo = newInvoiceNo,
                LedgerId = ledger.LedgerId,
                BranchId = request.BranchId,
                UserId = request.UserId,
                Date = request.Date ?? DateTime.Now,
                TotalAmount = request.TotalAmount,
                TotalGst = request.TotalGst,
                TotalDiscount = request.TotalDiscount,
                PaymentStatus = request.PaymentStatus,
                PaymentMode = request.PaymentMode,
                Balance = request.Balance,
                Received = request.Received
            };

            context.Sales.Add(sale);
            await context.SaveChangesAsync();

            // ✅ Step 4: Create SaleDetails
            foreach (var detail in request.SaleDetails)
            {
                var saleDetail = new SaleDetail
                {
                    SaleId = sale.SaleId,
                    ItemId = detail.ItemId,
                    Hsncode = detail.Hsncode,
                    Quantity = detail.Quantity,
                    Rate = detail.Rate,
                    Amount = detail.Amount,
                    DiscountPercentage = detail.DiscountPercentage,
                    DiscountAmount = detail.DiscountAmount,
                    Gstpercentage = detail.Gstpercentage,
                    Gstamount = detail.Gstamount,
                    Total = detail.Total
                };

                context.SaleDetails.Add(saleDetail);
            }

            await context.SaveChangesAsync();

            // ✅ Step 5: Create Transaction
            var transaction = new TransactionDatum
            {
                VoucherNo = sale.SaleId.ToString(),
                Date = sale.Date ?? DateTime.Now,
                PayMode = sale.PaymentMode,
                LedgerId = ledger.LedgerId,
                Total = sale.TotalAmount,
                Balance = sale.Balance,
                Received = sale.Received,
                TransactionMode = "Sale"
            };

            context.TransactionData.Add(transaction);
            await context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Sale created successfully",
                LedgerId = ledger.LedgerId,
                SaleId = sale.SaleId,
                InvoiceNo = newInvoiceNo
            });
        }














        [HttpGet("GetSaleDataById/{saleId}")]
        public async Task<IActionResult> GetSaleDataById(int saleId)
        {
            var saleData = await context.Sales
                .Where(s => s.SaleId == saleId)
                .Select(s => new
                {
                    s.SaleId,
                    s.InvoiceNo,
                    s.Date,
                    s.TotalAmount,
                    s.BranchId,
                    s.LedgerId,
                    LedgerName = s.Ledger != null ? s.Ledger.LedgerName : null,
                    BillingAddress = s.Ledger != null ? s.Ledger.BillingAddress : null,
                    ShippingAddress = s.Ledger != null ? s.Ledger.ShipingAddress : null,
                    Contact = s.Ledger != null ? s.Ledger.MobileNumber : null,
                    s.UserId,
                    s.TotalGst,
                    s.TotalDiscount,
                    s.PaymentStatus,
                    s.PaymentMode,
                    s.Balance,
                    s.Received,
                    SaleDetails = s.SaleDetails.Select(sd => new
                    {
                        sd.SaleDetailId,
                        sd.ItemId,
                        sd.Hsncode,
                        sd.Quantity,
                        sd.Rate,
                        sd.Amount,
                        sd.DiscountPercentage,
                        sd.DiscountAmount,
                        sd.Gstpercentage,
                        sd.Gstamount,
                        sd.Total,
                        ItemName = sd.Item != null ? sd.Item.ItemName : null,
                        CategoryId = sd.Item != null ? sd.Item.CategoryId : null,
                        //CategoryName = sd.Item != null && sd.Item.CategoryId != null ? sd.Item.CategoryId.Category : null, 
                        CategoryName = sd.Item != null && sd.Item.Category != null ? sd.Item.Category.CategoryName : null,

                        Unit = sd.Item != null ? sd.Item.Unit : null
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (saleData == null)
            {
                return NotFound(new { message = "Sale not found" });
            }

            // Return new object with totalBalance included
            var response = new
            {
                saleData.SaleId,
                saleData.InvoiceNo,
                saleData.Date,
                saleData.TotalAmount,
                saleData.BranchId,
                saleData.LedgerId,
                saleData.LedgerName,
                saleData.BillingAddress,
                saleData.ShippingAddress,
                saleData.Contact,
                saleData.UserId,
                saleData.TotalGst,
                saleData.TotalDiscount,
                saleData.PaymentStatus,
                saleData.PaymentMode,
                saleData.Balance,
                saleData.Received,
                saleData.SaleDetails
            };

            return Ok(response);
        }






        [HttpGet("GetUpdateSale/{ledgerId}")]
        public async Task<IActionResult> GetUpdateSale(int ledgerId)
        {
           
            // ✅ Get the OpeningBalance from the Ledger table
            var ledger = await context.Ledgers
                .FirstOrDefaultAsync(l => l.LedgerId == ledgerId);

            decimal LedgerAmount = ledger?.OpeningBalance ?? 0;



            decimal totalSaleBalance = await context.TransactionData
               .Where(r => r.LedgerId == ledgerId && r.TransactionMode == "Sale")
               .SumAsync(r => (decimal?)r.Balance) ?? 0;


            decimal totalPurchaseBalance = await context.TransactionData
               .Where(r => r.LedgerId == ledgerId && r.TransactionMode == "Purchase")
               .SumAsync(r => (decimal?)r.Balance) ?? 0;


            DateTime currentDate = DateTime.Now;


            var totalReceiptReceived = await context.TransactionData
                  .Where(r => r.LedgerId == ledgerId && r.Date <= currentDate && r.TransactionMode == "Receipt")
                  .SumAsync(r => (decimal?)r.Received) ?? 0;


            var totalPaymentReceived = await context.TransactionData
                  .Where(r => r.LedgerId == ledgerId && r.Date <= currentDate && r.TransactionMode == "Payment")
                  .SumAsync(r => (decimal?)r.Received) ?? 0;



            // ✅ Final total balance calculation
            decimal totalBalance = LedgerAmount + totalSaleBalance - totalReceiptReceived - totalPurchaseBalance + totalPaymentReceived;

            return Ok(new
            {
                TotalSaleBalance = totalSaleBalance,
                OpeningBalance = LedgerAmount,
                TotalReceiptCredits = totalReceiptReceived,
                TotalPurchaseBalance = totalPurchaseBalance,
                TotalPurchaseCredits = totalPaymentReceived,
                TotalBalance = totalBalance
            });

        }



        //[HttpPut("UpdateSale/{id}/{LedgerId}")]
        //public async Task<IActionResult> UpdateSale(int id,int LedgerId, [FromBody] Sale updatedSale)
        //{
        //    if (id != updatedSale.SaleId)
        //        return BadRequest("Sale ID mismatch");


        //    var existingSale = await context.Sales
        //       .Include(s => s.SaleDetails)
        //       .FirstOrDefaultAsync(s => s.SaleId == id);

        //    Ledger? ledger = null;
        //    if (request.LedgerId.HasValue && request.LedgerId > 0)
        //    {
        //        // Existing ledger
        //        ledger = await context.Ledgers.FirstOrDefaultAsync(l => l.LedgerId == request.LedgerId);
        //    }
        //    else
        //    {
        //        // New Ledger creation (you can take values from request or set defaults)
        //        ledger = new Ledger
        //        {
        //            LedgerName = request.Ledger?.LedgerName ?? "New Ledger",
        //            LedgerTypeId = '2',
        //            MobileNumber = request.Ledger?.MobileNumber,
        //            BillingAddress = request.Ledger?.BillingAddress,
        //            ShipingAddress = request.Ledger?.ShipingAddress,
        //            EmailId = request.Ledger?.EmailId,
        //            Gstnumber = request.Ledger?.Gstnumber,
        //            OpeningBalance = request.Ledger?.OpeningBalance ?? 0,
        //            OpeningBalanceDate = DateTime.Now
        //        };

        //        context.Ledgers.Add(ledger);
        //        await context.SaveChangesAsync();

        //        // Assign generated LedgerId back to request
        //        request.LedgerId = ledger.LedgerId;
        //    }


           

        //    if (existingSale == null)
        //        return NotFound("Sale not found");

        //    // Update Sale main properties
        //    existingSale.InvoiceNo = updatedSale.InvoiceNo;
        //    existingSale.LedgerId = updatedSale.LedgerId;
        //    existingSale.BranchId = updatedSale.BranchId;
        //    existingSale.UserId = updatedSale.UserId;
        //    existingSale.Date = updatedSale.Date;
        //    existingSale.TotalAmount = updatedSale.TotalAmount;
        //    existingSale.TotalGst = updatedSale.TotalGst;
        //    existingSale.TotalDiscount = updatedSale.TotalDiscount;
        //    existingSale.PaymentStatus = updatedSale.PaymentStatus;
        //    existingSale.PaymentMode = updatedSale.PaymentMode;
        //    existingSale.Balance = updatedSale.Balance;
        //    existingSale.Received = updatedSale.Received;

        //    // Sync SaleDetails
        //    var existingDetails = existingSale.SaleDetails.ToList();
        //    var incomingDetails = updatedSale.SaleDetails.ToList();

        //    // Remove deleted details
        //    foreach (var detail in existingDetails)
        //    {
        //        if (!incomingDetails.Any(d => d.SaleDetailId == detail.SaleDetailId))
        //        {
        //            context.SaleDetails.Remove(detail);
        //        }
        //    }

        //    // Add or update details
        //    foreach (var newDetail in incomingDetails)
        //    {
        //        var existingDetail = existingDetails
        //            .FirstOrDefault(d => d.SaleDetailId == newDetail.SaleDetailId);

        //        if (existingDetail != null)
        //        {
        //            // Update existing
        //            existingDetail.ItemId = newDetail.ItemId;
        //            existingDetail.Hsncode = newDetail.Hsncode;
        //            existingDetail.Quantity = newDetail.Quantity;
        //            existingDetail.Rate = newDetail.Rate;
        //            existingDetail.Amount = newDetail.Amount;
        //            existingDetail.DiscountPercentage = newDetail.DiscountPercentage;
        //            existingDetail.DiscountAmount = newDetail.DiscountAmount;
        //            existingDetail.Gstpercentage = newDetail.Gstpercentage;
        //            existingDetail.Gstamount = newDetail.Gstamount;
        //            existingDetail.Total = newDetail.Total;
        //        }
        //        else
        //        {
        //            // Add new
        //            var detailToAdd = new SaleDetail
        //            {
        //                SaleId = id,
        //                ItemId = newDetail.ItemId,
        //                Hsncode = newDetail.Hsncode,
        //                Quantity = newDetail.Quantity,
        //                Rate = newDetail.Rate,
        //                Amount = newDetail.Amount,
        //                DiscountPercentage = newDetail.DiscountPercentage,
        //                DiscountAmount = newDetail.DiscountAmount,
        //                Gstpercentage = newDetail.Gstpercentage,
        //                Gstamount = newDetail.Gstamount,
        //                Total = newDetail.Total
        //            };
        //            context.SaleDetails.Add(detailToAdd);
        //        }
        //    }

        //    // Recalculate Ledger Balance
        //    decimal totalLedgerBalance = await context.Sales
        //        .Where(s => s.LedgerId == updatedSale.LedgerId)
        //        .SumAsync(s => s.Balance ?? 0);

        //    var ledger = await context.Ledgers
        //        .FirstOrDefaultAsync(l => l.LedgerId == updatedSale.LedgerId);

        //    decimal openingBalance = ledger?.OpeningBalance ?? 0;
        //    decimal totalBalance = totalLedgerBalance + openingBalance;

        //    // Update or insert TransactionData
        //    var transaction = await context.TransactionData
        //        .FirstOrDefaultAsync(t => t.TransactionMode == "Sale" && t.VoucherNo == id.ToString());

        //    if (transaction != null)
        //    {
        //        transaction.Date = updatedSale.Date ?? DateTime.Now;
        //        transaction.PayMode = updatedSale.PaymentMode;
        //        transaction.LedgerId = updatedSale.LedgerId;
        //        transaction.Balance = updatedSale.Balance;
        //        transaction.Received = updatedSale.Received;
        //        transaction.Total = updatedSale.TotalAmount;
        //        transaction.TransactionMode = "Sale";
        //    }
        //    else
        //    {
        //        var newTransaction = new TransactionDatum
        //        {
        //            VoucherNo = id.ToString(),
        //            Date = updatedSale.Date ?? DateTime.Now,
        //            PayMode = updatedSale.PaymentMode,
        //            LedgerId = updatedSale.LedgerId,                 
        //            Balance = updatedSale.Balance,
        //            Received = updatedSale.Received,
        //            Total = updatedSale.TotalAmount,
        //            TransactionMode = "Sale"
        //        };
        //        context.TransactionData.Add(newTransaction);
        //    }

        //    await context.SaveChangesAsync();

        //    return Ok(new
        //    {
        //        Message = "Sale updated successfully.",
        //        SaleId = existingSale.SaleId,
        //        InvoiceNo = existingSale.InvoiceNo,
        //        TotalLedgerBalance = totalLedgerBalance,
        //        OpeningBalance = openingBalance,
        //        TotalBalance = totalBalance
        //    });
        //}

















        //[HttpPut("UpdateSale/{id}/{ledgerId}")]
        //public async Task<IActionResult> UpdateSale(int id, int ledgerId, [FromBody] Sale updatedSale)
        //{
        //    if (id != updatedSale.SaleId)
        //        return BadRequest("Sale ID mismatch");

        //    var existingSale = await context.Sales
        //        .Include(s => s.SaleDetails)
        //        .FirstOrDefaultAsync(s => s.SaleId == id);

        //    if (existingSale == null)
        //        return NotFound("Sale not found");

        //    // 🔹 Handle Ledger (Update or Create New)
        //    Ledger? ledger = null;
        //    if (ledgerId > 0)
        //    {
        //        ledger = await context.Ledgers.FirstOrDefaultAsync(l => l.LedgerId == ledgerId);
        //        if (ledger != null && updatedSale.Ledger != null)
        //        {
        //            // Update existing ledger info if provided 
        //            //ledger.LedgerName = updatedSale.Ledger.LedgerName ?? ledger.LedgerName;

        //            ledger.LedgerName = updatedSale.Ledger.LedgerName ?? ledger.LedgerName;
        //            ledger.MobileNumber = updatedSale.Ledger.MobileNumber ?? ledger.MobileNumber;
        //            ledger.BillingAddress = updatedSale.Ledger.BillingAddress ?? ledger.BillingAddress;
        //            ledger.ShipingAddress = updatedSale.Ledger.ShipingAddress ?? ledger.ShipingAddress;
        //            ledger.EmailId = updatedSale.Ledger.EmailId ?? ledger.EmailId;
        //            ledger.Gstnumber = updatedSale.Ledger.Gstnumber ?? ledger.Gstnumber;
        //        }
        //    }
        //    else
        //    {
        //        // Create new ledger
        //        ledger = new Ledger
        //        {
        //            LedgerName = updatedSale.Ledger?.LedgerName ?? "New Ledger",
        //            LedgerTypeId = 2,
        //            MobileNumber = updatedSale.Ledger?.MobileNumber,
        //            BillingAddress = updatedSale.Ledger?.BillingAddress,
        //            ShipingAddress = updatedSale.Ledger?.ShipingAddress,
        //            EmailId = updatedSale.Ledger?.EmailId,
        //            Gstnumber = updatedSale.Ledger?.Gstnumber,
        //            OpeningBalance = updatedSale.Ledger?.OpeningBalance ?? 0,
        //            OpeningBalanceDate = DateTime.Now
        //        };

        //        context.Ledgers.Add(ledger);
        //        await context.SaveChangesAsync();

        //        updatedSale.LedgerId = ledger.LedgerId;
        //    }

        //    // 🔹 Update Sale main properties
        //    existingSale.InvoiceNo = updatedSale.InvoiceNo;
        //    existingSale.LedgerId = updatedSale.LedgerId;
        //    existingSale.BranchId = updatedSale.BranchId;
        //    existingSale.UserId = updatedSale.UserId;
        //    existingSale.Date = updatedSale.Date;
        //    existingSale.TotalAmount = updatedSale.TotalAmount;
        //    existingSale.TotalGst = updatedSale.TotalGst;
        //    existingSale.TotalDiscount = updatedSale.TotalDiscount;
        //    existingSale.PaymentStatus = updatedSale.PaymentStatus;
        //    existingSale.PaymentMode = updatedSale.PaymentMode;
        //    existingSale.Balance = updatedSale.Balance;
        //    existingSale.Received = updatedSale.Received;

        //    // 🔹 Sync SaleDetails
        //    var existingDetails = existingSale.SaleDetails.ToList();
        //    var incomingDetails = updatedSale.SaleDetails.ToList();

        //    // Remove deleted details
        //    foreach (var detail in existingDetails)
        //    {
        //        if (!incomingDetails.Any(d => d.SaleDetailId == detail.SaleDetailId))
        //        {
        //            context.SaleDetails.Remove(detail);
        //        }
        //    }

        //    // Add or update details
        //    foreach (var newDetail in incomingDetails)
        //    {
        //        var existingDetail = existingDetails.FirstOrDefault(d => d.SaleDetailId == newDetail.SaleDetailId);
        //        if (existingDetail != null)
        //        {
        //            // Update existing
        //            existingDetail.ItemId = newDetail.ItemId;
        //            existingDetail.Hsncode = newDetail.Hsncode;
        //            existingDetail.Quantity = newDetail.Quantity;
        //            existingDetail.Rate = newDetail.Rate;
        //            existingDetail.Amount = newDetail.Amount;
        //            existingDetail.DiscountPercentage = newDetail.DiscountPercentage;
        //            existingDetail.DiscountAmount = newDetail.DiscountAmount;
        //            existingDetail.Gstpercentage = newDetail.Gstpercentage;
        //            existingDetail.Gstamount = newDetail.Gstamount;
        //            existingDetail.Total = newDetail.Total;
        //        }
        //        else
        //        {
        //            // Add new
        //            var detailToAdd = new SaleDetail
        //            {
        //                SaleId = id,
        //                ItemId = newDetail.ItemId,
        //                Hsncode = newDetail.Hsncode,
        //                Quantity = newDetail.Quantity,
        //                Rate = newDetail.Rate,
        //                Amount = newDetail.Amount,
        //                DiscountPercentage = newDetail.DiscountPercentage,
        //                DiscountAmount = newDetail.DiscountAmount,
        //                Gstpercentage = newDetail.Gstpercentage,
        //                Gstamount = newDetail.Gstamount,
        //                Total = newDetail.Total
        //            };
        //            context.SaleDetails.Add(detailToAdd);
        //        }
        //    }

        //    // 🔹 Recalculate Ledger Balance
        //    decimal totalLedgerBalance = await context.Sales
        //        .Where(s => s.LedgerId == updatedSale.LedgerId)
        //        .SumAsync(s => s.Balance ?? 0);

        //    decimal openingBalance = ledger?.OpeningBalance ?? 0;
        //    decimal totalBalance = totalLedgerBalance + openingBalance;

        //    // 🔹 Update or insert TransactionData
        //    var transaction = await context.TransactionData
        //        .FirstOrDefaultAsync(t => t.TransactionMode == "Sale" && t.VoucherNo == id.ToString());

        //    if (transaction != null)
        //    {
        //        transaction.Date = updatedSale.Date ?? DateTime.Now;
        //        transaction.PayMode = updatedSale.PaymentMode;
        //        transaction.LedgerId = updatedSale.LedgerId;
        //        transaction.Balance = updatedSale.Balance;
        //        transaction.Received = updatedSale.Received;
        //        transaction.Total = updatedSale.TotalAmount;
        //    }
        //    else
        //    {
        //        var newTransaction = new TransactionDatum
        //        {
        //            VoucherNo = id.ToString(),
        //            Date = updatedSale.Date ?? DateTime.Now,
        //            PayMode = updatedSale.PaymentMode,
        //            LedgerId = updatedSale.LedgerId,
        //            Balance = updatedSale.Balance,
        //            Received = updatedSale.Received,
        //            Total = updatedSale.TotalAmount,
        //            TransactionMode = "Sale"
        //        };
        //        context.TransactionData.Add(newTransaction);
        //    }

        //    await context.SaveChangesAsync();

        //    return Ok(new
        //    {
        //        Message = "Sale updated successfully.",
        //        SaleId = existingSale.SaleId,
        //        InvoiceNo = existingSale.InvoiceNo,
        //        TotalLedgerBalance = totalLedgerBalance,
        //        OpeningBalance = openingBalance,
        //        TotalBalance = totalBalance
        //    });
        //}










        [HttpPut("UpdateSale/{id}/{ledgerId}")]
        public async Task<IActionResult> UpdateSale(int id, int ledgerId, [FromBody] Sale updatedSale)
        {
            if (id != updatedSale.SaleId)
                return BadRequest("Sale ID mismatch");

            var existingSale = await context.Sales
                .Include(s => s.SaleDetails)
                .FirstOrDefaultAsync(s => s.SaleId == id);

            if (existingSale == null)
                return NotFound("Sale not found");

            // 🔹 Handle Ledger (Update or Create New)
            Ledger? ledger = null;

            // Check if we're dealing with the same ledger or a different one
            bool isSameLedger = existingSale.LedgerId == ledgerId;

            if (isSameLedger && ledgerId > 0)
            {
                // Same ledger - update existing
                ledger = await context.Ledgers.FirstOrDefaultAsync(l => l.LedgerId == ledgerId);
                if (ledger != null && updatedSale.Ledger != null)
                {
                    // Update existing ledger info
                    ledger.LedgerName = updatedSale.Ledger.LedgerName ?? ledger.LedgerName;
                    ledger.MobileNumber = updatedSale.Ledger.MobileNumber ?? ledger.MobileNumber;
                    ledger.BillingAddress = updatedSale.Ledger.BillingAddress ?? ledger.BillingAddress;
                    ledger.ShipingAddress = updatedSale.Ledger.ShipingAddress ?? ledger.ShipingAddress;
                    //ledger.EmailId = updatedSale.Ledger.EmailId ?? ledger.EmailId;
                    //ledger.Gstnumber = updatedSale.Ledger.Gstnumber ?? ledger.Gstnumber;

                    // Only update opening balance if explicitly provided
                    if (updatedSale.Ledger.OpeningBalance.HasValue)
                    {
                        ledger.OpeningBalance = updatedSale.Ledger.OpeningBalance.Value;
                    }
                }
            }
            else if (ledgerId > 0)
            {
                // Different existing ledger - use as is without updating
                ledger = await context.Ledgers.FirstOrDefaultAsync(l => l.LedgerId == ledgerId);
                updatedSale.LedgerId = ledgerId;
            }
            else
            {
                // Create new ledger
                ledger = new Ledger
                {
                    LedgerName = updatedSale.Ledger?.LedgerName ?? "New Ledger",
                    LedgerTypeId = 2,
                    MobileNumber = updatedSale.Ledger?.MobileNumber,
                    BillingAddress = updatedSale.Ledger?.BillingAddress,
                    ShipingAddress = updatedSale.Ledger?.ShipingAddress,
                    EmailId = updatedSale.Ledger?.EmailId,
                    Gstnumber = updatedSale.Ledger?.Gstnumber,
                    OpeningBalance = updatedSale.Ledger?.OpeningBalance ?? 0,
                    OpeningBalanceDate = DateTime.Now
                };

                context.Ledgers.Add(ledger);
                await context.SaveChangesAsync();

                updatedSale.LedgerId = ledger.LedgerId;
            }

            // 🔹 Update Sale main properties
            existingSale.InvoiceNo = updatedSale.InvoiceNo;
            existingSale.LedgerId = updatedSale.LedgerId;
            existingSale.BranchId = updatedSale.BranchId;
            existingSale.UserId = updatedSale.UserId;
            existingSale.Date = updatedSale.Date;
            existingSale.TotalAmount = updatedSale.TotalAmount;
            existingSale.TotalGst = updatedSale.TotalGst;
            existingSale.TotalDiscount = updatedSale.TotalDiscount;
            existingSale.PaymentStatus = updatedSale.PaymentStatus;
            existingSale.PaymentMode = updatedSale.PaymentMode;
            existingSale.Balance = updatedSale.Balance;
            existingSale.Received = updatedSale.Received;

            // 🔹 Sync SaleDetails
            var existingDetails = existingSale.SaleDetails.ToList();
            var incomingDetails = updatedSale.SaleDetails.ToList();

            // Remove deleted details
            foreach (var detail in existingDetails)
            {
                if (!incomingDetails.Any(d => d.SaleDetailId == detail.SaleDetailId))
                {
                    context.SaleDetails.Remove(detail);
                }
            }

            // Add or update details
            foreach (var newDetail in incomingDetails)
            {
                var existingDetail = existingDetails.FirstOrDefault(d => d.SaleDetailId == newDetail.SaleDetailId);
                if (existingDetail != null)
                {
                    // Update existing
                    existingDetail.ItemId = newDetail.ItemId;
                    existingDetail.Hsncode = newDetail.Hsncode;
                    existingDetail.Quantity = newDetail.Quantity;
                    existingDetail.Rate = newDetail.Rate;
                    existingDetail.Amount = newDetail.Amount;
                    existingDetail.DiscountPercentage = newDetail.DiscountPercentage;
                    existingDetail.DiscountAmount = newDetail.DiscountAmount;
                    existingDetail.Gstpercentage = newDetail.Gstpercentage;
                    existingDetail.Gstamount = newDetail.Gstamount;
                    existingDetail.Total = newDetail.Total;
                }
                else
                {
                    // Add new
                    var detailToAdd = new SaleDetail
                    {
                        SaleId = id,
                        ItemId = newDetail.ItemId,
                        Hsncode = newDetail.Hsncode,
                        Quantity = newDetail.Quantity,
                        Rate = newDetail.Rate,
                        Amount = newDetail.Amount,
                        DiscountPercentage = newDetail.DiscountPercentage,
                        DiscountAmount = newDetail.DiscountAmount,
                        Gstpercentage = newDetail.Gstpercentage,
                        Gstamount = newDetail.Gstamount,
                        Total = newDetail.Total
                    };
                    context.SaleDetails.Add(detailToAdd);
                }
            }

            // 🔹 Recalculate Ledger Balance
            decimal totalLedgerBalance = await context.Sales
                .Where(s => s.LedgerId == updatedSale.LedgerId)
                .SumAsync(s => s.Balance ?? 0);

            decimal openingBalance = ledger?.OpeningBalance ?? 0;
            decimal totalBalance = totalLedgerBalance + openingBalance;

            // 🔹 Update or insert TransactionData
            var transaction = await context.TransactionData
                .FirstOrDefaultAsync(t => t.TransactionMode == "Sale" && t.VoucherNo == id.ToString());

            if (transaction != null)
            {
                transaction.Date = updatedSale.Date ?? DateTime.Now;
                transaction.PayMode = updatedSale.PaymentMode;
                transaction.LedgerId = updatedSale.LedgerId;
                transaction.Balance = updatedSale.Balance;
                transaction.Received = updatedSale.Received;
                transaction.Total = updatedSale.TotalAmount;
            }
            else
            {
                var newTransaction = new TransactionDatum
                {
                    VoucherNo = id.ToString(),
                    Date = updatedSale.Date ?? DateTime.Now,
                    PayMode = updatedSale.PaymentMode,
                    LedgerId = updatedSale.LedgerId,
                    Balance = updatedSale.Balance,
                    Received = updatedSale.Received,
                    Total = updatedSale.TotalAmount,
                    TransactionMode = "Sale"
                };
                context.TransactionData.Add(newTransaction);
            }

            await context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Sale updated successfully.",
                SaleId = existingSale.SaleId,
                InvoiceNo = existingSale.InvoiceNo,
                TotalLedgerBalance = totalLedgerBalance,
                OpeningBalance = openingBalance,
                TotalBalance = totalBalance
            });
        }












        [HttpDelete("DeleteSale/{id}")]
        public IActionResult DeleteSale(int id)
        {

            var sale = context.Sales
                                  .Include(p => p.SaleDetails)
                                  .FirstOrDefault(x => x.SaleId == id);

            if (sale == null)
                return NotFound("Sale Data not found for delete.");

            context.SaleDetails.RemoveRange(sale.SaleDetails);

            // Then remove the main Sale record
            context.Sales.Remove(sale);

            context.SaveChanges();

            return Ok("Sale and its details deleted successfully.");
        }






        [HttpPost("CheckRawMaterials")]
        public IActionResult CheckRawMaterials([FromBody] List<int> rawMaterialIds)
        {
            if (rawMaterialIds == null || !rawMaterialIds.Any())
                return BadRequest("Please provide raw material IDs.");

            // Get distinct purchased RawMaterialIds
            var purchasedIds = context.PurchaseDetails
                .Where(pd => pd.RawMaterialId.HasValue)
                .Select(pd => pd.RawMaterialId.Value)
                .Distinct()
                .ToList();

            // Find missing IDs
            var missingIds = rawMaterialIds
                .Except(purchasedIds)
                .ToList();

            if (missingIds.Any())
            {
                // Get missing names
                var missingNames = context.RawMaterials
                    .Where(rm => missingIds.Contains(rm.RawMaterialId))
                    .Select(rm => rm.Name)
                    .ToList();

                return Ok(new
                {
                    Status = "Missing",
                    Message = $"Please purchase these raw materials: {string.Join(", ", missingNames)}",
                    MissingIds = missingIds,
                    MissingNames = missingNames
                });
            }

            return Ok(new
            {
                Status = "OK",
                Message = "All materials purchased"
            });
        }






        //[HttpGet("GetCurrentStock")]
        //public IActionResult GetCurrentStock()
        //{
        //    // Step 1: Get average rate for each RawMaterialId
        //    var avgRates = context.PurchaseDetails
        //        .GroupBy(p => p.RawMaterialId)
        //        .Select(g => new
        //        {
        //            RawMaterialId = g.Key,
        //            TotalAmount = g.Sum(x => (decimal?)x.Amount) ?? 0,
        //            TotalQty = g.Sum(x => (decimal?)x.Quantity) ?? 0,
        //            AverageRate = (g.Sum(x => (decimal?)x.Amount) ?? 0) / ((g.Sum(x => (decimal?)x.Quantity) ?? 1)),

        //        })
        //        .ToList();

        //    // Step 2: Fetch raw materials + related data
        //    var rawData = context.RawMaterials
        //        .Select(rm => new
        //        {
        //            RawMaterialId = rm.RawMaterialId,
        //            RawMaterialName = rm.Name,
        //            PurchaseUnit = rm.PurchaseUnit ?? "Unit",
        //            ConsumptionUnit = rm.ConsumptionUnit ?? "Unit",
        //            EquivalentUnitStr = rm.EquivalentUnit,

        //            OpeningStock = rm.OpeningStock ?? 0,

        //            CategoryName = context.RawMaterialCategories
        //                                .Where(c => c.CategoryId == rm.CategoryId)
        //                                .Select(c => c.CategoryName)
        //                                .FirstOrDefault() ?? "Uncategorized",

        //            PurchasedQty = context.PurchaseDetails
        //                .Where(pd => pd.RawMaterialId == rm.RawMaterialId)
        //                .Sum(pd => (decimal?)pd.Quantity) ?? 0,

        //            ConsumedQty = (
        //                from rd in context.RecipeDetails
        //                join r in context.Recipes on rd.RecipeId equals r.RecipeId
        //                join sd in context.SaleDetails on r.ItemId equals sd.ItemId
        //                where rd.RawMaterialId == rm.RawMaterialId
        //                select (decimal?)rd.Quantity * sd.Quantity
        //            ).Sum() ?? 0
        //        })
        //        .ToList(); // In-memory

        //    // Step 3: Build final report with AvgRate merged
        //    //var report = rawData.Select(r =>
        //    //{
        //    //    decimal equivalentUnit = 1;
        //    //    if (!string.IsNullOrWhiteSpace(r.EquivalentUnitStr) &&
        //    //        decimal.TryParse(r.EquivalentUnitStr, out var parsedValue) &&
        //    //        parsedValue != 0)
        //    //    {
        //    //        equivalentUnit = parsedValue;
        //    //    }

        //    //    var usedQty = r.ConsumedQty / equivalentUnit;
        //    //    var currentStock = r.OpeningStock + r.PurchasedQty - usedQty;

        //    //    // Get the average rate for this RawMaterialId
        //    //    var avg = avgRates.FirstOrDefault(a => a.RawMaterialId == r.RawMaterialId);

        //    //    return new
        //    //    {
        //    //        r.RawMaterialName,
        //    //        r.CategoryName,
        //    //        r.PurchaseUnit,
        //    //        r.ConsumptionUnit,
        //    //        EquivalentUnit = equivalentUnit,

        //    //        OpeningStock = Math.Round(r.OpeningStock, 2),
        //    //        PurchasedQty = Math.Round(r.PurchasedQty, 2),
        //    //        UsedQty = Math.Round(usedQty, 2),
        //    //        CurrentStock = Math.Round(currentStock, 2),

        //    //        AverageRate = Math.Round(avg?.AverageRate ?? 0, 2),
        //    //        TotalAmount = Math.Round(avg?.TotalAmount ?? 0, 2),
        //    //        TotalQty = Math.Round(avg?.TotalQty ?? 0, 2)
        //    //    };
        //    //}).ToList();








        //    // Step 3: Build final report with AvgRate merged
        //    var report = rawData.Select(r =>
        //    {
        //        decimal equivalentUnit = 1;
        //        if (!string.IsNullOrWhiteSpace(r.EquivalentUnitStr) &&
        //            decimal.TryParse(r.EquivalentUnitStr, out var parsedValue) &&
        //            parsedValue != 0)
        //        {
        //            equivalentUnit = parsedValue;
        //        }

        //        var usedQty = r.ConsumedQty / equivalentUnit;
        //        var currentStock = r.OpeningStock + r.PurchasedQty - usedQty;

        //        // Get the average rate for this RawMaterialId
        //        var avg = avgRates.FirstOrDefault(a => a.RawMaterialId == r.RawMaterialId);
        //        var averageRate = avg?.AverageRate ?? 0;

        //        // Calculate Total Outstanding
        //        var totalOutstanding = currentStock * averageRate;

        //        //return new
        //        //{
        //        //    r.RawMaterialName,
        //        //    r.CategoryName,
        //        //    r.PurchaseUnit,
        //        //    r.ConsumptionUnit,
        //        //    EquivalentUnit = equivalentUnit,

        //        //    OpeningStock = Math.Round(r.OpeningStock, 2),
        //        //    PurchasedQty = Math.Round(r.PurchasedQty, 2),
        //        //    UsedQty = Math.Round(usedQty, 2),
        //        //    CurrentStock = Math.Round(currentStock, 2),

        //        //    AverageRate = Math.Round(averageRate, 2),
        //        //    TotalAmount = Math.Round(avg?.TotalAmount ?? 0, 2),
        //        //    TotalQty = Math.Round(avg?.TotalQty ?? 0, 2),

        //        //    // ✅ New field added
        //        //    TotalOutstanding = Math.Round(totalOutstanding, 2)
        //        //};
        //        return new
        //        {
        //            r.RawMaterialName,
        //            r.CategoryName,
        //            r.PurchaseUnit,
        //            r.ConsumptionUnit,
        //            EquivalentUnit = equivalentUnit.ToString("0.00"),

        //            OpeningStock = r.OpeningStock.ToString("0.00"),
        //            PurchasedQty = r.PurchasedQty.ToString("0.00"),
        //            UsedQty = usedQty.ToString("0.00"),
        //            CurrentStock = currentStock.ToString("0.00"),

        //            AverageRate = averageRate.ToString("0.00"),
        //            TotalAmount = (avg?.TotalAmount ?? 0).ToString("0.00"),
        //            TotalQty = (avg?.TotalQty ?? 0).ToString("0.00"),

        //            TotalOutstanding = totalOutstanding.ToString("0.00")
        //        };
        //    }).ToList();

        //    return Ok(report);
        //}









        [HttpGet("GetCurrentStock")]
        public IActionResult GetCurrentStock()
        {
            // Step 1: Get average rate for each RawMaterialId
            var avgRates = context.PurchaseDetails
                .GroupBy(p => p.RawMaterialId)
                .Select(g => new
                {
                    RawMaterialId = g.Key,
                    TotalAmount = g.Sum(x => (decimal?)x.Amount) ?? 0,
                    TotalQty = g.Sum(x => (decimal?)x.Quantity) ?? 0,
                    AverageRate = (g.Sum(x => (decimal?)x.Amount) ?? 0) / ((g.Sum(x => (decimal?)x.Quantity) ?? 1)),

                })
                .ToList();

            // Step 2: Fetch raw materials + related data
            var rawData = context.RawMaterials
                .Select(rm => new
                {
                    RawMaterialId = rm.RawMaterialId,
                    RawMaterialName = rm.Name,
                    PurchaseUnit = rm.PurchaseUnit ?? "Unit",
                    ConsumptionUnit = rm.ConsumptionUnit ?? "Unit",
                    EquivalentUnitStr = rm.EquivalentUnit,

                    YeildPercenatge = rm.YeildPercenatge,

                    OpeningStock = rm.OpeningStock ?? 0,


                    CategoryName = context.RawMaterialCategories
                                        .Where(c => c.CategoryId == rm.CategoryId)
                                        .Select(c => c.CategoryName)
                                        .FirstOrDefault() ?? "Uncategorized",

                    PurchasedQty = context.PurchaseDetails
                        .Where(pd => pd.RawMaterialId == rm.RawMaterialId)
                        .Sum(pd => (decimal?)pd.Quantity) ?? 0,


                    ConsumedQty = (
                        from rd in context.RecipeDetails
                        join r in context.Recipes on rd.RecipeId equals r.RecipeId
                        join sd in context.SaleDetails on r.ItemId equals sd.ItemId
                        where rd.RawMaterialId == rm.RawMaterialId
                        select (decimal?)rd.Quantity * sd.Quantity
                    ).Sum() ?? 0
                })
                .ToList(); // In-memory


            // Step 3: Build final report with AvgRate merged
            var report = rawData.Select(r =>
            {
                decimal equivalentUnit = 1;
                if (!string.IsNullOrWhiteSpace(r.EquivalentUnitStr) &&
                    decimal.TryParse(r.EquivalentUnitStr, out var parsedValue) &&
                    parsedValue != 0)
                {
                    equivalentUnit = parsedValue;
                }


                decimal yieldPercent = 100;
                if (!string.IsNullOrWhiteSpace(r.YeildPercenatge) &&
                    decimal.TryParse(r.YeildPercenatge, out var parsedYield) &&
                    parsedYield > 0)
                {
                    yieldPercent = parsedYield;
                }

                var usedQty = r.ConsumedQty / equivalentUnit;

                var usableOpening = r.OpeningStock * (yieldPercent / 100m);
                var usablePurchased = r.PurchasedQty * (yieldPercent / 100m);


                //var currentStock = r.OpeningStock + r.PurchasedQty - usedQty; 


                var currentStock = usablePurchased - usedQty;


                // Get the average rate for this RawMaterialId
                var avg = avgRates.FirstOrDefault(a => a.RawMaterialId == r.RawMaterialId);
                var averageRate = avg?.AverageRate ?? 0;

                // Calculate Total Outstanding
                var totalOutstanding = currentStock * averageRate;

               
                return new
                {
                    r.RawMaterialName,
                    r.CategoryName,
                    r.PurchaseUnit,
                    r.ConsumptionUnit,
                    EquivalentUnit = equivalentUnit.ToString("0.00"),

                    OpeningStock = r.OpeningStock.ToString("0.00"),
                    PurchasedQty = r.PurchasedQty.ToString("0.00"),
                    UsedQty = usedQty.ToString("0.00"),
                    CurrentStock = currentStock.ToString("0.00"),

                    AverageRate = averageRate.ToString("0.00"),
                    TotalAmount = (avg?.TotalAmount ?? 0).ToString("0.00"),
                    TotalQty = (avg?.TotalQty ?? 0).ToString("0.00"),

                    TotalOutstanding = totalOutstanding.ToString("0.00")
                };
            }).ToList();

            return Ok(report);
        }






        [HttpGet("StockSummaryReport")]
        public async Task<IActionResult> StockSummaryReport(DateTime? fromDate = null, DateTime? toDate = null, int? categoryId = null)
        {
            var today = DateTime.Now;
            var startOfMonth = new DateTime(today.Year, today.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

            fromDate ??= startOfMonth;
            toDate ??= endOfMonth;

            var query = context.RawMaterials.AsQueryable();

            // ✅ Apply category filter if provided
            if (categoryId.HasValue)
                query = query.Where(rm => rm.CategoryId == categoryId.Value);

            var data = await (from rm in query
                              select new StockSummary
                              {
                                  RawMaterialId = rm.RawMaterialId,
                                  RawMaterialName = rm.Name + " [" + rm.PurchaseUnit + "]",
                                  PurchaseUnit = rm.PurchaseUnit,
                                  ConsumptionUnit = rm.ConsumptionUnit,
                                  EquivalentUnit = rm.EquivalentUnit,
                                  YeildPercentage = rm.YeildPercenatge,  // string

                                  Opening = (rm.OpeningStockDate < fromDate ? (rm.OpeningStock ?? 0) : 0),

                                  // ✅ Purchases
                                  Purchase = rm.PurchaseDetails
                                               .Where(pd => pd.Purchase != null &&
                                                            pd.Purchase.Date >= fromDate &&
                                                            pd.Purchase.Date <= toDate)
                                               .Sum(pd => pd.Quantity ?? 0),

                                  // ✅ Excess
                                  Excess = rm.ManageStocks
                                             .Where(ms => ms.Date >= fromDate && ms.Date <= toDate)
                                             .Sum(ms => (ms.ClosingQuantity ?? 0) + ((ms.SubQuantity ?? 0) / 1000m)),

                                  // ✅ Wastage
                                  Wastage = rm.WastageDetails
                                              .Where(wd => wd.Wastage != null &&
                                                           wd.Wastage.Date >= fromDate &&
                                                           wd.Wastage.Date <= toDate)
                                              .Sum(wd => wd.Quantity ?? 0),

                                  // ✅ Consumed
                                  Consumed = rm.RecipeDetails
                                               .SelectMany(rd => rd.Recipe.Item.SaleDetails
                                                   .Where(sd => sd.Sale != null &&
                                                                sd.Sale.Date >= fromDate &&
                                                                sd.Sale.Date <= toDate)
                                                   .Select(sd => (rd.Quantity ?? 0) * (sd.Quantity ?? 0))
                                               )
                                               .Sum(),

                                  Production = rm.ProductionDetails
                                                  .Where(pd => pd.Production != null &&
                                                   pd.Production.Date >= fromDate &&
                                                   pd.Production.Date <= toDate)
                                                .Sum(pd => pd.Quantity ?? 0),


                                  Transfer = 0,
                                  Shortage = 0,
                                  //  Production = 0
                              }).ToListAsync();

            // ✅ Convert yield percentage string → decimal, then calculate UsableQty
            var result = data.Select(q =>
            {
                decimal yield = 100; // default if null/invalid
                if (!string.IsNullOrEmpty(q.YeildPercentage))
                    decimal.TryParse(q.YeildPercentage, out yield);

                var usableQty = (q.Purchase * yield) / 100;

                return new
                {
                    q.RawMaterialId,
                    q.RawMaterialName,
                    q.PurchaseUnit,
                    q.ConsumptionUnit,
                    q.EquivalentUnit,

                    Opening = string.Format("{0:0.00}", q.Opening),
                    Purchase = string.Format("{0:0.00}", q.Purchase),
                    Excess = string.Format("{0:0.00}", q.Excess),

                    Consumed = q.Consumed,
                    ConsumedFormatted = q.ConsumedFormatted,

                    Wastage = q.Wastage,
                    WastageFormatted = q.WastageFormatted,

                    YeildPercenatge = yield,     // now decimal value
                    UsableQty = string.Format("{0:0.00}", usableQty),

                    Transfer = string.Format("{0:0.00}", q.Transfer),
                    Shortage = string.Format("{0:0.00}", q.Shortage),
                    Production = q.Production, 

                    ConversionFormatted = q.ConversionFormatted
                };
            });

            return Ok(result);
        }





      


        [HttpGet("GetProductionDetails")]
        public async Task<IActionResult> GetProductionDetails(int rawMaterialId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var details = await context.ProductionDetails
                .Where(pd => pd.RawMaterialId == rawMaterialId &&
                             pd.Production != null &&
                             pd.Production.Date >= fromDate &&
                             pd.Production.Date <= toDate)
                .Select(pd => new
                {
                    InvoiceDate = pd.Production.Date,
                    Qty = pd.Quantity,
                    Unit = pd.Unit,
                })
                .ToListAsync();

            // Calculate total summary string without changing the existing structure
            var totalSummaryString = string.Join(", ",
                details
                    .GroupBy(d => d.Unit)
                    .Select(g => $"{g.Sum(x => x.Qty ?? 0)} {g.Key}")
            );

            // Return anonymous object with both details and total summary
            return Ok(new
            {
                Details = details,
                TotalSummary = totalSummaryString
            });
        }




        [HttpGet("GetPurchaseDetails")]
        public async Task<IActionResult> GetPurchaseDetails(int rawMaterialId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var details = await context.PurchaseDetails
                .Where(pd => pd.RawMaterialId == rawMaterialId &&
                             pd.Purchase != null &&
                             pd.Purchase.Date >= fromDate &&
                             pd.Purchase.Date <= toDate)
                .Select(pd => new
                {
                    InvoiceDate = pd.Purchase.Date,
                    Qty = pd.Quantity,
                    Unit = pd.Unit,
                    Rate=pd.Rate,
                    Amount = pd.Amount
                })
                .ToListAsync();

            // Calculate total summary string without changing the existing structure
            var totalSummaryString = string.Join(", ",
                details
                    .GroupBy(d => d.Unit)
                    .Select(g => $"{g.Sum(x => x.Qty ?? 0)} {g.Key}")
            );

            // Return anonymous object with both details and total summary
            return Ok(new
            {
                Details = details,
                TotalSummary = totalSummaryString
            });
        }



        [HttpGet("GetWastageDetails")]
        public async Task<IActionResult> GetWastageDetails(int rawMaterialId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var details = await context.WastageDetails
                .Where(pd => pd.RawMaterialId == rawMaterialId &&
                             pd.Wastage != null &&
                             pd.Wastage.Date >= fromDate &&
                             pd.Wastage.Date <= toDate)
                .Select(pd => new
                {
                    InvoiceDate = pd.Wastage.Date,
                    Qty = pd.Quantity,
                    Unit = pd.Unit,
                    AvgPurchasePrice=pd.AvgPurchasePrice,
                    Amount = pd.Amount
                })
                .ToListAsync();

            // Calculate total summary string without changing the existing structure
            var totalSummaryString = string.Join(", ",
                details
                    .GroupBy(d => d.Unit)
                    .Select(g => $"{g.Sum(x => x.Qty ?? 0)} {g.Key}")
            );

            // Return anonymous object with both details and total summary
            return Ok(new
            {
                Details = details,
                TotalSummary = totalSummaryString
            });
        }



     



        [HttpGet("GetConsumedDetails")]
        public async Task<IActionResult> GetConsumedDetails(int rawMaterialId, DateTime? fromDate = null, DateTime? toDate = null)
        {
           

            // ✅ Query: Get all sales where the raw material is used in recipes
            var details = await context.RecipeDetails
                .Where(rd => rd.RawMaterialId == rawMaterialId)
                .SelectMany(rd => rd.Recipe.Item.SaleDetails
                    .Where(sd => sd.Sale != null &&
                                 sd.Sale.Date >= fromDate &&
                                 sd.Sale.Date <= toDate)
                    .Select(sd => new
                    {
                        InvoiceDate = sd.Sale.Date,
                        Qty = (rd.Quantity ?? 0) * (sd.Quantity ?? 0),  // Total consumed = RecipeQty * SaleQty
                        Unit = rd.Unit,
                        SaleQuantity=sd.Quantity,
                        Rate = sd.Rate, 
                        Amount=sd.Amount
                       // Amount = ((rd.Quantity ?? 0) * (sd.Quantity ?? 0)) * (rd.RawMaterial.AvgPurchasePrice ?? 0)
                    })
                )
                .ToListAsync();

            // ✅ Build total summary by unit
            var totalSummaryString = string.Join(", ",
                details
                    .GroupBy(d => d.Unit)
                    .Select(g => $"{g.Sum(x => x.Qty):0.00} {g.Key}")
            );

            // ✅ Return details + total summary
            return Ok(new
            {
                Details = details,
                TotalSummary = totalSummaryString
            });
        }




        //[HttpGet("StockSummaryReport")]
        //public async Task<IActionResult> StockSummaryReport(DateTime? fromDate = null, DateTime? toDate = null, int? categoryId = null)
        //{
        //    var today = DateTime.Now;
        //    var startOfMonth = new DateTime(today.Year, today.Month, 1);
        //    var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

        //    fromDate ??= startOfMonth;
        //    toDate ??= endOfMonth;

        //    var query = context.RawMaterials.AsQueryable();

        //    // Apply category filter if provided
        //    if (categoryId.HasValue)
        //        query = query.Where(rm => rm.CategoryId == categoryId.Value);

        //    var data = await (from rm in query
        //                      select new StockSummary
        //                      {
        //                          RawMaterialName = rm.Name,
        //                          PurchaseUnit = rm.PurchaseUnit,
        //                          ConsumptionUnit = rm.ConsumptionUnit,
        //                          EquivalentUnit = rm.EquivalentUnit,
        //                          YeildPercentage = rm.YeildPercenatge,

        //                          Opening = (rm.OpeningStockDate < fromDate ? (rm.OpeningStock ?? 0) : 0),

        //                          Purchase1 = rm.PurchaseDetails
        //                                       .Where(pd => pd.Purchase != null &&
        //                                                    pd.Purchase.Date >= fromDate &&
        //                                                    pd.Purchase.Date <= toDate)
        //                                       .Sum(pd => pd.Quantity ?? 0),

        //                          Excess = rm.ManageStocks
        //                                     .Where(ms => ms.Date >= fromDate && ms.Date <= toDate)
        //                                     .Sum(ms => (ms.ClosingQuantity ?? 0) + ((ms.SubQuantity ?? 0) / 1000m)),

        //                          Wastage = rm.WastageDetails
        //                                      .Where(wd => wd.Wastage != null &&
        //                                                   wd.Wastage.Date >= fromDate &&
        //                                                   wd.Wastage.Date <= toDate)
        //                                      .Sum(wd => wd.Quantity ?? 0),

        //                          Consumed = rm.RecipeDetails
        //                                       .SelectMany(rd => rd.Recipe.Item.SaleDetails
        //                                           .Where(sd => sd.Sale != null &&
        //                                                        sd.Sale.Date >= fromDate &&
        //                                                        sd.Sale.Date <= toDate)
        //                                           .Select(sd => (rd.Quantity ?? 0) * (sd.Quantity ?? 0))
        //                                       )
        //                                       .Sum(),

        //                          Production = rm.ProductionDetails
        //                                          .Where(pd => pd.Production != null &&
        //                                                       pd.Production.Date >= fromDate &&
        //                                                       pd.Production.Date <= toDate)
        //                                          .Sum(pd => pd.Quantity ?? 0),

        //                          Transfer = 0,
        //                          Shortage = 0,
        //                      }).ToListAsync();

        //    // Apply Yield and Unit Conversion to all numeric values
        //    var result = data.Select(q =>
        //    {
        //        // 1️⃣ Parse Yield %
        //        decimal yield = 100;
        //        if (!string.IsNullOrEmpty(q.YeildPercentage))
        //            decimal.TryParse(q.YeildPercentage, out yield);

        //        // 2️⃣ Parse EquivalentUnit
        //        decimal equivalent = 1;
        //        if (!string.IsNullOrEmpty(q.EquivalentUnit))
        //            decimal.TryParse(q.EquivalentUnit, out equivalent);

        //        // 3️⃣ Apply yield to Purchase
        //        decimal usableQty = (q.Purchase1 * yield) / 100;

        //        // 4️⃣ Convert all numeric values to PurchaseUnit
        //        decimal opening = q.Opening;
        //        decimal purchase = q.Purchase1;
        //        decimal excess = q.Excess;
        //        decimal consumed = q.Consumed;
        //        decimal wastage = q.Wastage;
        //        decimal production = q.Production;
        //    //    decimal yeildpercentage = q.YeildPercenatge; 



        //        string displayUnit = q.PurchaseUnit ?? "Unit";

        //        if (!string.IsNullOrEmpty(q.PurchaseUnit) &&
        //            !string.IsNullOrEmpty(q.ConsumptionUnit) &&
        //            !string.IsNullOrEmpty(q.EquivalentUnit) &&
        //            !q.PurchaseUnit.Equals(q.ConsumptionUnit, StringComparison.OrdinalIgnoreCase))
        //        {
        //            // Convert smaller unit → purchase unit
        //            opening /= equivalent;
        //          //  purchase /= equivalent;
        //            excess /= equivalent;
        //            consumed /= equivalent;
        //            wastage /= equivalent;
        //            production /= equivalent;
        //            //  yeildpercentage /= equivalent;
        //            usableQty /= equivalent;

        //              displayUnit = q.PurchaseUnit;
        //        }

        //        return new
        //        {
        //            q.RawMaterialName,
        //            PurchaseUnit = displayUnit,
        //            q.ConsumptionUnit,
        //            EquivalentUnit = equivalent.ToString("0.##"),

        //            Opening = opening.ToString("0.00"),
        //            Purchase1 = purchase,
        //            Excess = excess.ToString("0.00"),
        //            Consumed = consumed.ToString("0.00"),
        //            Wastage = wastage.ToString("0.00"),
        //            YeildPercenatge = yield.ToString("0.00"),
        //            UsableQty = usableQty.ToString("0.00"),
        //            Transfer = q.Transfer.ToString("0.00"),
        //            Shortage = q.Shortage.ToString("0.00"),
        //            Production = production.ToString("0.00")
        //        };
        //    });

        //    return Ok(result);
        //}











        [HttpGet("GetStockSummary")]
        public IActionResult GetStockSummary(DateTime? fromDate, DateTime? toDate)
        {
            // Step 1: Get average rate for each RawMaterialId
            var filteredPurchaseDetails = context.PurchaseDetails.AsQueryable();

            if (fromDate.HasValue && toDate.HasValue)
            {
                filteredPurchaseDetails = filteredPurchaseDetails
                    .Where(pd => pd.Purchase != null && pd.Purchase.Date >= fromDate && pd.Purchase.Date <= toDate);
            }

            var avgRates = filteredPurchaseDetails
                .GroupBy(p => p.RawMaterialId)
                .Select(g => new
                {
                    RawMaterialId = g.Key,
                    TotalAmount = g.Sum(x => (decimal?)x.Amount) ?? 0,
                    TotalQty = g.Sum(x => (decimal?)x.Quantity) ?? 0,
                    AverageRate = (g.Sum(x => (decimal?)x.Amount) ?? 0) / ((g.Sum(x => (decimal?)x.Quantity) ?? 1)),
                })
                .ToList();

            // Step 2: Fetch raw materials + related data
            var rawData = context.RawMaterials
                .Select(rm => new
                {
                    RawMaterialId = rm.RawMaterialId,
                    RawMaterialName = rm.Name,
                    PurchaseUnit = rm.PurchaseUnit ?? "Unit",
                    ConsumptionUnit = rm.ConsumptionUnit ?? "Unit",
                    EquivalentUnitStr = rm.EquivalentUnit,
                    OpeningStock = rm.OpeningStock ?? 0,

                    CategoryName = context.RawMaterialCategories
                                        .Where(c => c.CategoryId == rm.CategoryId)
                                        .Select(c => c.CategoryName)
                                        .FirstOrDefault() ?? "Uncategorized",

                    // 🔄 Filter purchase quantity by date if provided
                    PurchasedQty = context.PurchaseDetails
                        .Where(pd => pd.RawMaterialId == rm.RawMaterialId &&
                            (!fromDate.HasValue || (pd.Purchase != null && pd.Purchase.Date >= fromDate)) &&
                            (!toDate.HasValue || (pd.Purchase != null && pd.Purchase.Date <= toDate)))
                        .Sum(pd => (decimal?)pd.Quantity) ?? 0,

                    // 🔄 Filter consumed quantity by sale date if provided
                    ConsumedQty = (
                        from rd in context.RecipeDetails
                        join r in context.Recipes on rd.RecipeId equals r.RecipeId
                        join sd in context.SaleDetails on r.ItemId equals sd.ItemId
                        join s in context.Sales on sd.SaleId equals s.SaleId
                        where rd.RawMaterialId == rm.RawMaterialId &&
                            (!fromDate.HasValue || (s.Date != null && s.Date >= fromDate)) &&
                            (!toDate.HasValue || (s.Date != null && s.Date <= toDate))
                        select (decimal?)rd.Quantity * sd.Quantity
                    ).Sum() ?? 0
                })
                .ToList();

            // Step 3: Build final report
            var report = rawData.Select(r =>
            {
                decimal equivalentUnit = 1;
                if (!string.IsNullOrWhiteSpace(r.EquivalentUnitStr) &&
                    decimal.TryParse(r.EquivalentUnitStr, out var parsedValue) &&
                    parsedValue != 0)
                {
                    equivalentUnit = parsedValue;
                }

                var usedQty = r.ConsumedQty / equivalentUnit;
                var currentStock = r.OpeningStock + r.PurchasedQty;

                var avg = avgRates.FirstOrDefault(a => a.RawMaterialId == r.RawMaterialId);
                var averageRate = avg?.AverageRate ?? 0;
                var totalOutstanding = currentStock * averageRate;

                var totalOpeningAndPurchase = r.OpeningStock + r.PurchasedQty;
                var difference = usedQty - totalOpeningAndPurchase;

                //return new
                //{
                //    r.RawMaterialName,
                //    r.CategoryName,
                //    r.PurchaseUnit,
                //    r.ConsumptionUnit,
                //    EquivalentUnit = equivalentUnit,

                //    OpeningStock = Math.Round(r.OpeningStock, 2),
                //    Purchase = Math.Round(r.PurchasedQty, 2),
                //    UsedQty = Math.Round(usedQty, 2),
                //    Consumption = Math.Round(currentStock, 2),

                //    AverageRate = Math.Round(averageRate, 2),
                //    TotalAmount = Math.Round(avg?.TotalAmount ?? 0, 2),
                //    TotalQty = Math.Round(avg?.TotalQty ?? 0, 2),
                //    TotalOutstanding = Math.Round(totalOutstanding, 2),

                //    TotalOpeningandPurchase = Math.Round(totalOpeningAndPurchase, 2),
                //    Difference = Math.Round(difference, 2)
                //};



                return new
                {
                    r.RawMaterialName,
                    r.CategoryName,
                    r.PurchaseUnit,
                    r.ConsumptionUnit,
                    EquivalentUnit = equivalentUnit.ToString("0.00"),

                    OpeningStock = r.OpeningStock.ToString("0.00"),
                    Purchase = r.PurchasedQty.ToString("0.00"),
                    UsedQty = usedQty.ToString("0.00"),
                    Consumption = currentStock.ToString("0.00"),

                    AverageRate = averageRate.ToString("0.00"),
                    TotalAmount = (avg?.TotalAmount ?? 0).ToString("0.00"),
                    TotalQty = (avg?.TotalQty ?? 0).ToString("0.00"),
                    TotalOutstanding = totalOutstanding.ToString("0.00"),

                    TotalOpeningandPurchase = totalOpeningAndPurchase.ToString("0.00"),
                    Difference = difference.ToString("0.00")
                };


            }).ToList();

            return Ok(report);
        }



        [HttpGet("ReceipeCostingReport")]
        public async Task<IActionResult> ReceipeCostingReport()
        {
            var data = await (
                from recipe in context.Recipes
                join item in context.Items on recipe.ItemId equals item.ItemId
                join category in context.ItemCategories on item.CategoryId equals category.CategoryId
                select new
                {
                    recipe.RecipeId,
                    item.ItemName,
                    category.CategoryName,
                    SellingPrice = item.SalePrice ?? 0,

                    RecipeDetails = (
                        from rd in context.RecipeDetails
                        join rm in context.RawMaterials on rd.RawMaterialId equals rm.RawMaterialId
                        join pdg in
                            (from pd in context.PurchaseDetails
                             group pd by pd.RawMaterialId into g
                             select new
                             {
                                 RawMaterialId = g.Key,
                                 TotalPurchaseQuantity = g.Sum(x => x.Quantity) ?? 0,
                                 TotalRate = g.Sum(x => x.Rate) ?? 0,
                                 AvgRate = g.Average(x => x.Rate) ?? 0,
                                 Unit = g.Select(x => x.Unit).FirstOrDefault()
                             }) on rd.RawMaterialId equals pdg.RawMaterialId
                        join cu in context.ComparisonUnits
                            on new { FromUnit = rm.PurchaseUnit, ToUnit = rm.ConsumptionUnit }
                            equals new { cu.FromUnit, cu.ToUnit } into cuJoin
                        from cu in cuJoin.DefaultIfEmpty()

                        where rd.RecipeId == recipe.RecipeId
                        select new
                        {
                            Singlecost = cu != null && pdg.TotalPurchaseQuantity > 0
                                ? Math.Round(
                                    (decimal)(rd.Quantity ?? 0) *
                                    (pdg.TotalRate / (pdg.TotalPurchaseQuantity * decimal.Parse(cu.EquivalentValue))),
                                    2
                                  )
                                : 0
                        }).ToList()
                }).ToListAsync();

            var result = data.Select(r =>
            {
                var totalRecipeCost = r.RecipeDetails.Sum(d => d.Singlecost);
                var marginPercentage = r.SellingPrice != 0
                    ? Math.Round(((r.SellingPrice - totalRecipeCost) / r.SellingPrice) * 100, 2)
                    : 0;

                return new
                {
                    r.RecipeId,
                    r.ItemName,
                    r.CategoryName,
                    RecipeCosting = totalRecipeCost.ToString("0.00"),
                    SellingPrice = r.SellingPrice.ToString("0.00"),
                    MarginPercentage = marginPercentage.ToString("0.00")
                };
            })
            .OrderBy(r => r.ItemName)
            .ToList();

            return Ok(result);
        }



        [HttpGet("ReceipeCostingCalculation")]
        public async Task<IActionResult> ReceipeCostingCalculation(int receipeid)
        {
            // Step 1: Get top 15 raw material IDs
            var topRawMaterialIds = await context.PurchaseDetails
                .GroupBy(pd => pd.RawMaterialId)
                .Select(g => new
                {
                    RawMaterialId = g.Key,
                    TotalRate = g.Sum(x => x.Rate)
                })
                .OrderByDescending(g => g.TotalRate)
                .Take(15)
                .Select(g => g.RawMaterialId)
                .ToListAsync();

            // Step 2: Pull raw data from DB
            var rawData = await (
                from recipe in context.Recipes
                join item in context.Items on recipe.ItemId equals item.ItemId
                join category in context.ItemCategories on item.CategoryId equals category.CategoryId
                where recipe.RecipeId == receipeid
                select new
                {
                    recipe.RecipeId,
                    item.ItemName,
                    category.CategoryName,
                    RecipeDetails = (
                        from rd in context.RecipeDetails
                        join rm in context.RawMaterials on rd.RawMaterialId equals rm.RawMaterialId
                        join pdg in
                            (from pd in context.PurchaseDetails
                             group pd by pd.RawMaterialId into g
                             select new
                             {
                                 RawMaterialId = g.Key,
                                 TotalPurchaseQuantity = g.Sum(x => x.Quantity) ?? 0,
                                 TotalRate = g.Sum(x => x.Rate) ?? 0,
                                 AvgRate = g.Average(x => x.Rate) ?? 0,
                                 Unit = g.Select(x => x.Unit).FirstOrDefault()
                             }) on rd.RawMaterialId equals pdg.RawMaterialId
                        join cu in context.ComparisonUnits
                            on new { FromUnit = rm.PurchaseUnit, ToUnit = rm.ConsumptionUnit }
                            equals new { cu.FromUnit, cu.ToUnit } into cuJoin
                        from cu in cuJoin.DefaultIfEmpty()
                        where rd.RecipeId == recipe.RecipeId
                              && topRawMaterialIds.Contains(rd.RawMaterialId)
                        select new
                        {
                            rd.RecipeDetailsId,
                            RawMaterialName = rm.Name,
                            ConsumptionUnit = rm.ConsumptionUnit,
                            EquivalentValue = cu.EquivalentValue,
                            FromUnit = cu.FromUnit,
                            ToUnit = cu.ToUnit,
                            rd.Quantity,
                            rd.Unit,
                            PurchaseQuantity = pdg.TotalPurchaseQuantity,
                            PurchaseUnit = pdg.Unit,
                            Rate = pdg.TotalRate
                        }).ToList()
                }).ToListAsync();

            // Step 3: Calculate Singlecost in memory and format numbers
            var result = rawData.Select(r => new
            {
                r.RecipeId,
                r.ItemName,
                r.CategoryName,
                RecipeDetails = r.RecipeDetails.Select(d =>
                {
                    decimal eqValue = 1;
                    if (!string.IsNullOrEmpty(d.EquivalentValue))
                        decimal.TryParse(d.EquivalentValue, out eqValue);

                    var singleCost = (eqValue > 0 && d.PurchaseQuantity > 0)
                        ? Math.Round(
                            (d.Quantity ?? 0) * (d.Rate / (d.PurchaseQuantity * eqValue)),
                            2
                          )
                        : 0;

                    return new
                    {
                        d.RecipeDetailsId,
                        d.RawMaterialName,
                        ConsumptionUnit = d.ConsumptionUnit,
                        Equivalent = eqValue > 0 ? $"1 {d.FromUnit} = {eqValue} {d.ToUnit}" : null,
                        Quantity = (d.Quantity ?? 0).ToString("0.00"),
                        d.Unit,
                        PurchaseQuantity = d.PurchaseQuantity.ToString("0.00"),
                        d.PurchaseUnit,
                        Rate = d.Rate.ToString("0.00"),
                        Singlecost = singleCost.ToString("0.00")
                    };
                }).ToList()
            });

            return Ok(result);
        }





        //[HttpGet("PurchaseReport")]
        //public async Task<IActionResult> PurchaseReport()
        //{
        //    var data = context.Purchases.ToList();
        //    return Ok();
        //}




        //[HttpGet("PurchaseReport")]
        //public async Task<IActionResult> GetPurchaseItemReport(DateTime fromDate, DateTime toDate)
        //{
        //    var data = await context.PurchaseDetails
        //        .Include(pd => pd.Purchase)
        //        .Include(pd => pd.RawMaterial)
        //        .ThenInclude(r => r.Category)
        //        .Where(pd => pd.Purchase.Date >= fromDate && pd.Purchase.Date <= toDate)
        //        .GroupBy(pd => new { pd.RawMaterial.Name, pd.RawMaterial.PurchaseUnit, pd.RawMaterial.Category.CategoryName })
        //        .Select(g => new
        //        {
        //            Category = g.Key.CategoryName,
        //            RawMaterial = g.Key.Name + " [" + g.Key.PurchaseUnit + "]",
        //            TotalQuantity = g.Sum(x => x.Quantity ?? 0),
        //            TotalPrice = g.Sum(x => x.Total ?? 0),
        //            MinPrice = g.Min(x => x.Rate ?? 0),
        //            MaxPrice = g.Max(x => x.Rate ?? 0),
        //            AvgPrice = g.Average(x => x.Rate ?? 0)
        //        })
        //        .ToListAsync();

        //    return Ok(data);
        //}






        [HttpGet("PurchaseReport")]
        public async Task<IActionResult> GetPurchaseReport(
            DateTime? fromDate = null,
            DateTime? toDate = null,
            string? categoryName = null,
            string? rawMaterial = null)
        {
            var query = context.PurchaseDetails
                .Include(pd => pd.Purchase)
                .Include(pd => pd.RawMaterial)
                .ThenInclude(r => r.Category)
                .AsQueryable();

            // ✅ Apply filters if provided
            if (fromDate.HasValue && toDate.HasValue)
            {
                query = query.Where(pd => pd.Purchase.Date >= fromDate && pd.Purchase.Date <= toDate);
            }

            if (!string.IsNullOrEmpty(categoryName))
            {
                query = query.Where(pd => pd.RawMaterial.Category.CategoryName.Contains(categoryName));
            }


            if (!string.IsNullOrEmpty(rawMaterial))
            {
                query = query.Where(pd => pd.RawMaterial.Name.Contains(rawMaterial));
            }

            // ✅ Grouping & Projection
            var data = await query
                .GroupBy(pd => new { pd.RawMaterial.RawMaterialId, pd.RawMaterial.Name, pd.RawMaterial.PurchaseUnit, pd.RawMaterial.Category.CategoryName })
                .Select(g => new
                {
                    RawMaterialId=g.Key.RawMaterialId,
                    Category = g.Key.CategoryName,
                    RawMaterial = g.Key.Name + " [" + g.Key.PurchaseUnit + "]",
                    //TotalQuantity = g.Sum(x => x.Quantity ?? 0),
                    //TotalPrice = g.Sum(x => x.Total ?? 0),
                    //MinPrice = g.OrderByDescending(x => x.Rate ?? 0).Take(15).Min(x => x.Rate ?? 0),
                    //MaxPrice = g.OrderByDescending(x => x.Rate ?? 0).Take(15).Max(x => x.Rate ?? 0),
                    //AvgPrice = g.Average(x => x.Rate ?? 0)



                    //TotalQuantity = Math.Round(g.Sum(x => x.Quantity ?? 0), 2),
                    //TotalPrice = Math.Round(g.Sum(x => x.Total ?? 0), 2),
                    //MinPrice = Math.Round(g.OrderByDescending(x => x.Rate ?? 0).Take(15).Min(x => x.Rate ?? 0), 2),
                    //MaxPrice = Math.Round(g.OrderByDescending(x => x.Rate ?? 0).Take(15).Max(x => x.Rate ?? 0), 2),
                    //AvgPrice = Math.Round(g.Average(x => x.Rate ?? 0), 2)



                    TotalQuantity = g.Sum(x => x.Quantity ?? 0).ToString("F2"),
                    TotalPrice = g.Sum(x => x.Total ?? 0).ToString("F2"),
                    MinPrice = g.OrderByDescending(x => x.Rate ?? 0).Take(15).Min(x => x.Rate ?? 0).ToString("F2"),
                    MaxPrice = g.OrderByDescending(x => x.Rate ?? 0).Take(15).Max(x => x.Rate ?? 0).ToString("F2"),
                    AvgPrice = ((g.OrderByDescending(x => x.Rate ?? 0).Take(15).Min(x => x.Rate ?? 0) +
                          g.OrderByDescending(x => x.Rate ?? 0).Take(15).Max(x => x.Rate ?? 0)) / 2).ToString("F2")


                })
                .ToListAsync();

            return Ok(data);
        }







        [HttpGet("GetPurchaseDetails/{rawmaterialId}")]
        public async Task<IActionResult> GetPurchaseDetails(int rawmaterialId)
        {
            var details = await context.PurchaseDetails
                .Where(pd => pd.RawMaterialId== rawmaterialId )
                 .OrderByDescending(pd => pd.Purchase.Date)
                .Select(pd => new
                {
                    Date = pd.Purchase.Date.HasValue
                            ? pd.Purchase.Date.Value.ToString("dd MMM yyyy")
                            : null,
                    From = pd.Purchase.Ledger != null ? pd.Purchase.Ledger.LedgerName : null,
                    Quantity = pd.Quantity,
                    Price = pd.Rate,
                    WeightedPrice = pd.Quantity.HasValue && pd.Rate.HasValue
                                        ? (pd.Quantity.Value * pd.Rate.Value)
                                        : 0,
                    Tax = pd.Gstamount ?? 0,
                    Total = pd.Total ?? 0
                })
                .ToListAsync();

            if (!details.Any())
            {
                return NotFound($"No details found for this RwaMaterial");
            }

            return Ok(details);
        }




        [HttpGet("GetClosingStockByDate/{date}")]
        public async Task<IActionResult> GetClosingStockByDate(DateTime date)
        {
            var stocks = await context.ManageStocks
                .Include(c => c.RawMaterial)
                .ThenInclude(r => r.Category)
                .Where(c => c.Date == date.Date)
                .Select(c => new
                {
                    Category = c.RawMaterial.Category.CategoryName,
                    RawMaterial = c.RawMaterial.Name,
                    ClosingQuantity = c.ClosingQuantity,
                    Unit = c.Unit,
                    Comments = c.Comments
                })
                .ToListAsync();

            return Ok(stocks);
        }








        //[HttpGet("GetWastage/{rawMaterialId}")]
        //public async Task<IActionResult> GetRawMaterialDetails(int rawMaterialId)
        //{
        //    var data = await (
        //        from rm in context.RawMaterials
        //        where rm.RawMaterialId == rawMaterialId

        //        join pdg in
        //            (from pd in context.PurchaseDetails
        //             group pd by pd.RawMaterialId into g
        //             select new
        //             {
        //                 RawMaterialId = g.Key,
        //                 TotalPurchaseQuantity = g.Sum(x => x.Quantity),
        //                 TotalRate = g.Sum(x => x.Rate),
        //                 AvgRate = g.Average(x => x.Rate),
        //                 Unit = g.Select(x => x.Unit).FirstOrDefault()
        //             }) on rm.RawMaterialId equals pdg.RawMaterialId into pdgJoin
        //        from pdg in pdgJoin.DefaultIfEmpty()

        //        join rd in context.RecipeDetails
        //            on rm.RawMaterialId equals rd.RawMaterialId into rdJoin
        //        from rd in rdJoin.DefaultIfEmpty()

        //        join cu in context.ComparisonUnits
        //            on new { FromUnit = rm.PurchaseUnit, ToUnit = rm.ConsumptionUnit }
        //            equals new { cu.FromUnit, cu.ToUnit } into cuJoin
        //        from cu in cuJoin.DefaultIfEmpty()

        //        select new
        //        {
        //            // RawMaterial Master Fields
        //            rm.RawMaterialId,
        //            rm.Name,
        //            rm.PurchaseUnit,
        //            rm.ConsumptionUnit,
        //            rm.EquivalentUnit,
        //            rm.PurchasePrice,
        //            rm.TransferPrice,
        //            rm.ReconciliationPrice,
        //            rm.OpeningStock,
        //            rm.OpeningStockPrice,
        //            rm.OpeningStockDate,
        //            rm.MinStockToMaintain,
        //            rm.YeildPercenatge,

        //            // Purchase Details (aggregated)
        //            PurchaseQuantity = pdg.TotalPurchaseQuantity,
        //            PurchaseUnit1 = pdg.Unit,
        //            AvgRate = pdg.AvgRate,
        //            TotalRate = pdg.TotalRate,

        //            // Recipe Detail (if used in recipe)
        //            RecipeQuantity = rd != null ? rd.Quantity : 0,
        //            RecipeUnit = rd != null ? rd.Unit : null,

        //            // Conversion
        //            Equivalent = cu != null
        //                ? "1 " + cu.FromUnit + " = " + cu.EquivalentValue + " " + cu.ToUnit
        //                : null,

        //            // Calculation (SingleCost)
        //            SingleCost = (rd != null && pdg.TotalPurchaseQuantity != null && cu != null)
        //                ? Math.Round(
        //                    (decimal)(rd.Quantity ?? 0) *
        //                    ((decimal)pdg.TotalRate / ((decimal)pdg.TotalPurchaseQuantity * decimal.Parse(cu.EquivalentValue))),
        //                    2
        //                  )
        //                : 0
        //        }
        //    ).FirstOrDefaultAsync();

        //    return Ok(data);
        //}






        [HttpPost("CreateWastage")]
        public async Task<IActionResult> CreateWastage([FromBody] Wastage request)
        {
            if (request == null || request.WastageDetails == null || !request.WastageDetails.Any())
            {
                return BadRequest("Wastage and at least one Wastage is required.");
            }

         
            // ✅ Step 3: Create Wastage
            var wastage = new Wastage
            {
                Date = request.Date

            };

            context.Wastages.Add(wastage);
            await context.SaveChangesAsync();

            // ✅ Step 4: Create WastageDetails
            foreach (var detail in request.WastageDetails)
            {
                var wastageDetail = new WastageDetail
                {
                    WastageId = wastage.WastageId,
                    RawMaterialId= detail.RawMaterialId,
                    Quantity = detail.Quantity,
                    Unit=detail.Unit,
                    AvgPurchasePrice=detail.AvgPurchasePrice,
                    Amount = detail.Amount,
                    Description=detail.Description
                };

                context.WastageDetails.Add(wastageDetail);
                await context.SaveChangesAsync();
            }


            return Ok(new
            {
                Message = "Wastage created successfully" 
            });
        }










        [HttpGet("GetTop15RawMaterials")]
        public async Task<IActionResult> GetTop15RawMaterials()
        {
            // Step 1: Get top 15 raw material IDs by total purchase cost (Rate)
            var topRawMaterialIds = await context.PurchaseDetails
                .GroupBy(pd => pd.RawMaterialId)
                .Select(g => new
                {
                    RawMaterialId = g.Key,
                    TotalRate = g.Sum(x => x.Rate)
                })
                .OrderByDescending(g => g.TotalRate)
                .Take(15)
                .Select(g => g.RawMaterialId)
                .ToListAsync();

            // Step 2: For those top 15, calculate AvgPurchasePrice
            var data = await (
                from rm in context.RawMaterials
                join pdg in
                    (from pd in context.PurchaseDetails
                     group pd by pd.RawMaterialId into g
                     select new
                     {
                         RawMaterialId = g.Key,
                         AvgPurchasePrice = g.Average(x => x.Rate),
                         TotalRate = g.Sum(x => x.Rate),
                         TotalPurchaseQuantity = g.Sum(x => x.Quantity),
                         Unit = g.Select(x => x.Unit).FirstOrDefault()
                     }) on rm.RawMaterialId equals pdg.RawMaterialId
                where topRawMaterialIds.Contains(rm.RawMaterialId)
                select new
                {
                    rm.RawMaterialId,
                    RawMaterialName = rm.Name,
                    pdg.AvgPurchasePrice,
                    pdg.TotalRate,
                    pdg.TotalPurchaseQuantity,
                    pdg.Unit
                }
            ).ToListAsync();

            return Ok(data);
        }



        //[HttpGet("GetStock")]
        //public async Task<IActionResult> GetStock(DateTime? date = null)
        //{
        //    if (date.HasValue)
        //    {
        //        // Case 1: Specific date
        //        var stocks = await (from rm in context.RawMaterials
        //                            join cat in context.RawMaterialCategories
        //                                on rm.CategoryId equals cat.CategoryId
        //                            join ms in context.ManageStocks
        //                                .Where(s => s.Date == date.Value.Date)
        //                                on rm.RawMaterialId equals ms.RawMaterialId into msGroup
        //                            from stock in msGroup.DefaultIfEmpty()
        //                            select new
        //                            {
        //                                Category = cat.CategoryName,
        //                                RawMaterial = rm.Name,
                                        
        //                                ClosingQuantity = stock.ClosingQuantity ?? 0,
        //                                SubQuantity= stock.SubQuantity ?? 0,
        //                                Unit = (stock.Unit ?? (rm.PurchaseUnit + "," + rm.ConsumptionUnit)),
        //                                PurchaseUnit = rm.PurchaseUnit,
        //                                ConsumptionUnit = rm.ConsumptionUnit,
        //                                DisplayQuantity =
        //                                                  (stock.ClosingQuantity ?? 0) + " " + (stock.RawMaterial.PurchaseUnit ?? "") +
        //                                                      ", " + (stock.SubQuantity ?? 0) + " " + (stock.RawMaterial.ConsumptionUnit ?? ""),
        //                                Comments = stock.Comments ?? ""
        //                            }).ToListAsync();

        //        return Ok(stocks);
        //    }
        //    else
        //    {
        //        // Case 2: Latest stock or 0
        //        var stocks = await (from rm in context.RawMaterials
        //                            join cat in context.RawMaterialCategories
        //                                on rm.CategoryId equals cat.CategoryId
        //                            let latestStock = context.ManageStocks
        //                                .Where(s => s.RawMaterialId == rm.RawMaterialId)
        //                                .OrderByDescending(s => s.Date)
        //                                .FirstOrDefault()
        //                            select new
        //                            {
        //                                Category = cat.CategoryName,
        //                                RawMaterial = rm.Name,
        //                                DisplayQuantity =
        //                                                  (latestStock.ClosingQuantity ?? 0) + " " + (latestStock.RawMaterial.PurchaseUnit ?? "") +
        //                                                      ", " + (latestStock.SubQuantity ?? 0) + " " + (latestStock.RawMaterial.ConsumptionUnit ?? ""),
        //                                ClosingQuantity = latestStock.ClosingQuantity ?? 0,
        //                                SubQuantity = latestStock.SubQuantity ?? 0,
        //                                Unit = latestStock != null ? latestStock.Unit
        //                                    : (rm.PurchaseUnit + "," + rm.ConsumptionUnit),
        //                                PurchaseUnit = rm.PurchaseUnit,
        //                                ConsumptionUnit = rm.ConsumptionUnit,
        //                                Comments = latestStock.Comments ?? ""

        //                            }).ToListAsync();

        //        return Ok(stocks);
        //    }
        //}










        [HttpGet("GetStock")]
        public async Task<IActionResult> GetStock(DateTime? date = null)
        {
            if (date.HasValue)
            {
                // Case 1: Specific date
                var stocks = await (from rm in context.RawMaterials
                                    join cat in context.RawMaterialCategories
                                        on rm.CategoryId equals cat.CategoryId
                                    join ms in context.ManageStocks
                                        .Where(s => s.Date == date.Value.Date)
                                        on rm.RawMaterialId equals ms.RawMaterialId into msGroup
                                    from stock in msGroup.DefaultIfEmpty()
                                    select new
                                    {
                                        RawMaterialId=rm.RawMaterialId,
                                        Category = cat.CategoryName,
                                        RawMaterial = rm.Name,

                                        ClosingQuantity = stock.ClosingQuantity ?? 0,
                                        SubQuantity = stock.SubQuantity ?? 0,
                                        Unit = stock.Unit ?? (rm.PurchaseUnit + "," + rm.ConsumptionUnit),
                                        PurchaseUnit = rm.PurchaseUnit,
                                        ConsumptionUnit = rm.ConsumptionUnit,

                                        // ✅ use rm.PurchaseUnit / rm.ConsumptionUnit (not stock.RawMaterial)
                                        DisplayQuantity =
                                            (stock.ClosingQuantity ?? 0) + " " + (rm.PurchaseUnit ?? "") +
                                            ", " + (stock.SubQuantity ?? 0) + " " + (rm.ConsumptionUnit ?? ""),

                                        Comments = stock.Comments ?? ""
                                    }).ToListAsync();

                return Ok(stocks);
            }
            else
            {
                // Case 2: Latest stock or 0
                var stocks = await (from rm in context.RawMaterials
                                    join cat in context.RawMaterialCategories
                                        on rm.CategoryId equals cat.CategoryId
                                    let latestStock = context.ManageStocks
                                        .Where(s => s.RawMaterialId == rm.RawMaterialId)
                                        .OrderByDescending(s => s.Date)
                                        .FirstOrDefault()
                                    select new
                                    {
                                        RawMaterialId = rm.RawMaterialId,
                                        Category = cat.CategoryName,
                                        RawMaterial = rm.Name,

                                        ClosingQuantity = latestStock != null ? (latestStock.ClosingQuantity ?? 0) : 0,
                                        SubQuantity = latestStock != null ? (latestStock.SubQuantity ?? 0) : 0,
                                        Unit = latestStock != null ? latestStock.Unit : (rm.PurchaseUnit + "," + rm.ConsumptionUnit),
                                        PurchaseUnit = rm.PurchaseUnit,
                                        ConsumptionUnit = rm.ConsumptionUnit,

                                        // ✅ null-safe check
                                        DisplayQuantity = latestStock != null
                                            ? ((latestStock.ClosingQuantity ?? 0) + " " + (rm.PurchaseUnit ?? "") +
                                               ", " + (latestStock.SubQuantity ?? 0) + " " + (rm.ConsumptionUnit ?? ""))
                                            : ("0 " + (rm.PurchaseUnit ?? "") + ", 0 " + (rm.ConsumptionUnit ?? "")),

                                        Comments = latestStock != null ? latestStock.Comments ?? "" : ""
                                    }).ToListAsync();

                return Ok(stocks);
            }
        }











        [HttpPost("InsertStock")]
        public async Task<IActionResult> InsertStock([FromBody] ManageStock request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Check if record already exists for same RawMaterialId + Date
            var exists = await context.ManageStocks
                .AnyAsync(s => s.RawMaterialId == request.RawMaterialId && s.Date.Value.Date == request.Date);

            if (exists)
            {
                return BadRequest(new { Message = "Stock already exists for this date. Please update instead." });
            }

            // Insert new record
            var stock = new ManageStock
            {
                RawMaterialId = request.RawMaterialId,
                Date = request.Date,
                ClosingQuantity = request.ClosingQuantity,  // change to string in model if you want "20,500"
                SubQuantity=request.SubQuantity,
                Unit = request.Unit,
                Comments = request.Comments
            };

            context.ManageStocks.Add(stock);
            await context.SaveChangesAsync();

            return Ok(new { Message = "Stock inserted successfully", StockId = stock.ClosingStockId });
        }






        //[HttpPost("CreatetStock")]
        //public async Task<IActionResult> CreatetStock([FromBody] ManageStock request)
        //{
        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);

        //    // Check if record already exists for same RawMaterialId + Date
        //    var existingStock = await context.ManageStocks
        //        .FirstOrDefaultAsync(s => s.RawMaterialId == request.RawMaterialId
        //                                  && s.Date.HasValue
        //                                  && s.Date == request.Date);

        //    if (existingStock != null)
        //    {
        //        // ✅ Update existing record
        //        existingStock.ClosingQuantity = request.ClosingQuantity;
        //        existingStock.SubQuantity = request.SubQuantity;
        //        existingStock.Unit = request.Unit;
        //        existingStock.Comments = request.Comments;

        //        context.ManageStocks.Update(existingStock);
        //        await context.SaveChangesAsync();

        //        return Ok(new { Message = "Stock updated successfully", StockId = existingStock.ClosingStockId });
        //    }

        //    // ✅ Insert new record
        //    var stock = new ManageStock
        //    {
        //        RawMaterialId = request.RawMaterialId,
        //        Date = request.Date,
        //        ClosingQuantity = request.ClosingQuantity,
        //        SubQuantity = request.SubQuantity,
        //        Unit = request.Unit,
        //        Comments = request.Comments
        //    };

        //    context.ManageStocks.Add(stock);
        //    await context.SaveChangesAsync();

        //    return Ok(new { Message = "Stock inserted successfully", StockId = stock.ClosingStockId });
        //}










        [HttpPost("CreatetStock")]
        public async Task<IActionResult> CreatetStock([FromBody] List<ManageStock> requests)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            foreach (var request in requests)
            {
                var existingStock = await context.ManageStocks
                    .FirstOrDefaultAsync(s => s.RawMaterialId == request.RawMaterialId
                                              && s.Date.HasValue
                                              && s.Date.Value.Date == request.Date.Value.Date);

                if (existingStock != null)
                {
                    // ✅ Update existing record
                    existingStock.ClosingQuantity = request.ClosingQuantity;
                    existingStock.SubQuantity = request.SubQuantity;
                    existingStock.Unit = request.Unit;
                    existingStock.Comments = request.Comments;

                    context.ManageStocks.Update(existingStock);
                }
                else
                {
                    // ✅ Insert new record
                    var stock = new ManageStock
                    {
                        RawMaterialId = request.RawMaterialId,
                        Date = request.Date,
                        ClosingQuantity = request.ClosingQuantity,
                        SubQuantity = request.SubQuantity,
                        Unit = request.Unit,
                        Comments = request.Comments
                    };

                    context.ManageStocks.Add(stock);
                }
            }

            await context.SaveChangesAsync();

            return Ok(new { Message = "Stocks processed successfully" });
        }






        [HttpGet ("GetSupplierReport")]
        public async Task<IActionResult> GetSupplierReport(DateTime? fromDate = null, DateTime? toDate = null, int? supplierId = null)
        {
            var query = context.TransactionData
                .Include(t => t.Ledger) // Ledger = Supplier
                .AsQueryable();


            query = query.Where(t => t.TransactionMode == "Purchase");

            // ✅ Apply filters only if provided
            if (fromDate.HasValue)
                query = query.Where(t => t.Date >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(t => t.Date <= toDate.Value);

            if (supplierId.HasValue)
                query = query.Where(t => t.LedgerId == supplierId.Value);

            // ✅ Group by Supplier and calculate totals (returning anonymous type)
            var report = await query
                .GroupBy(t => new { t.LedgerId, t.Ledger.LedgerName })
                .Select(g => new
                {
                    SupplierId = g.Key.LedgerId,
                    SupplierName = g.Key.LedgerName,
                    TotalPurchase = g.Sum(x => x.Total ?? 0),
                    RemainingAmount = g.Sum(x => x.Balance ?? 0),
                    TotalPaid = g.Sum(x => x.Received ?? 0)
                })
                .ToListAsync();

            return Ok(report);
        }




        [HttpGet("SupplierInvoices/{ledgerId}")]
        public async Task<IActionResult> GetSupplierInvoices(int ledgerId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var query = context.Purchases
                .Where(p => p.LedgerId == ledgerId);

            // filter by date range if provided
            if (fromDate.HasValue && toDate.HasValue)
                query = query.Where(p => p.Date >= fromDate && p.Date <= toDate);

            var invoices = await query
                .Select(p => new
                {
                    p.PurchaseId,
                    p.InvoiceNo,
                    p.Date,
                    p.TotalAmount,
                    p.TotalDiscount,
                    p.TotalGst,
                    p.Balance,
                    p.Received
                })
                .OrderByDescending(p => p.Date)
                .ToListAsync();

            return Ok(invoices);
        }





        [HttpGet("SupplierInvoicesDetails/{purchaseId}")]
        public async Task<IActionResult> SupplierInvoicesDetails(int purchaseId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var query = context.PurchaseDetails
                .Where(p => p.PurchaseId == purchaseId); 


            var invoices = await query
                .Select(p => new
                {
                    p.PurchaseDetailId,
                    p.PurchaseId,
                    p.Quantity,
                    p.Rate,
                    p.Gstamount,
                    p.Total

                })
                .OrderByDescending(p => p.PurchaseDetailId)
                .ToListAsync();

            return Ok(invoices);
        }




        //[HttpGet("InvoiceRawMaterials/{purchaseId}")]
        //public async Task<IActionResult> GetInvoiceRawMaterials(int purchaseId, DateTime? fromDate = null,DateTime? toDate = null)
        //{
        //    var rawMaterials = await context.PurchaseDetails
        //        .Include(d => d.RawMaterial)
        //        .ThenInclude(r => r.Category)
        //        .Where(d => d.PurchaseId == purchaseId)
        //        .Select(d => new
        //        {
        //            Category = d.RawMaterial.Category.CategoryName,
        //            RawMaterial = d.RawMaterial.Name,
        //            Quantity = d.Quantity ?? 0,
        //            Price = d.Rate ?? 0,
        //            WeightedPrice = (d.Quantity ?? 0) * (d.Rate ?? 0),
        //            Tax = d.Gstamount ?? 0,
        //            Total = d.Total ?? 0
        //        })
        //        .ToListAsync();

        //    return Ok(rawMaterials);
        //}





        [HttpGet("InvoiceRawMaterials/{purchaseId}")]
        public async Task<IActionResult> GetInvoiceRawMaterials(int purchaseId,DateTime? fromDate = null,DateTime? toDate = null)
        {
            var query = context.PurchaseDetails
                .Include(d => d.RawMaterial)
                .ThenInclude(r => r.Category)
                .Where(d => d.PurchaseId == purchaseId);

            // ✅ Apply date filter via parent Purchase
            if (fromDate.HasValue && toDate.HasValue)
            {
                query = query.Where(d =>
                    d.Purchase.Date >= fromDate &&
                    d.Purchase.Date <= toDate);
            }

            var rawMaterials = await query
                .Select(d => new
                {
                    Category = d.RawMaterial.Category.CategoryName,
                    RawMaterial = d.RawMaterial.Name,
                    Quantity = d.Quantity ?? 0,
                    Price = d.Rate ?? 0,
                    WeightedPrice = (d.Quantity ?? 0) * (d.Rate ?? 0),
                    Tax = d.Gstamount ?? 0,
                    Total = d.Total ?? 0
                })
                .ToListAsync();

            return Ok(rawMaterials);
        }





        //[HttpGet("StockSummaryReport")]
        //public async Task<IActionResult> StockSummaryReport(DateTime? fromDate = null, DateTime? toDate = null)
        //{
        //    // Default to current month
        //    var today = DateTime.Now;
        //    var startOfMonth = new DateTime(today.Year, today.Month, 1);
        //    var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

        //    fromDate ??= startOfMonth;
        //    toDate ??= endOfMonth;

        //    var query = await (from rm in context.RawMaterials
        //                       select new
        //                       {
        //                           RawMaterialName = rm.Name + " [" + rm.PurchaseUnit + "]",

        //                           Opening = string.Format("{0:0.000}", rm.OpeningStock ?? 0),

        //                           // ✅ Purchases in date range
        //                           Purchase = string.Format("{0:0.000}",
        //                               rm.PurchaseDetails
        //                                 .Where(pd => pd.Purchase != null &&
        //                                              pd.Purchase.Date >= fromDate &&
        //                                              pd.Purchase.Date <= toDate)
        //                                 .Sum(pd => pd.Quantity ?? 0)
        //                           ),

        //                           // ✅ Excess = ClosingQuantity (kg) + SubQuantity (gm converted to kg)
        //                           Excess = string.Format("{0:0.000}",
        //                               rm.ManageStocks
        //                                 .Where(ms => ms.Date >= fromDate && ms.Date <= toDate)
        //                                 .Sum(ms =>
        //                                     (ms.ClosingQuantity ?? 0) +
        //                                     ((ms.SubQuantity ?? 0) / 1000.0m) // grams → kg
        //                                 )
        //                           ),

        //                           // ✅ Others set to 0.000
        //                           Consumed = string.Format("{0:0.000}", 0),
        //                           Wastage = string.Format("{0:0.000}", 0),
        //                           NormalLoss = string.Format("{0:0.000}", 0),
        //                           Transfer = string.Format("{0:0.000}", 0),
        //                           Shortage = string.Format("{0:0.000}", 0),
        //                           Conversion = string.Format("{0:0.000}", 0)
        //                       }).ToListAsync();

        //    return Ok(query);
        //}





        //[HttpGet("StockSummaryReport")]
        //public async Task<IActionResult> StockSummaryReport(DateTime? fromDate = null, DateTime? toDate = null)
        //{
        //    var today = DateTime.Now;
        //    var startOfMonth = new DateTime(today.Year, today.Month, 1);
        //    var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

        //    fromDate ??= startOfMonth;
        //    toDate ??= endOfMonth;

        //    var query = await (from rm in context.RawMaterials
        //                       select new StockSummary
        //                       {
        //                           RawMaterialName = rm.Name + " [" + rm.PurchaseUnit + "]",

        //                           PurchaseUnit = rm.PurchaseUnit,
        //                           EquivalentUnit = rm.EquivalentUnit,

        //                           Opening = rm.OpeningStock ?? 0,

        //                           // ✅ Purchases in date range
        //                           Purchase = rm.PurchaseDetails
        //                                        .Where(pd => pd.Purchase != null &&
        //                                                     pd.Purchase.Date >= fromDate &&
        //                                                     pd.Purchase.Date <= toDate)
        //                                        .Sum(pd => pd.Quantity ?? 0),

        //                           // ✅ Excess = ClosingQuantity + SubQuantity (gm→kg example)
        //                           Excess = rm.ManageStocks
        //                                      .Where(ms => ms.Date >= fromDate && ms.Date <= toDate)
        //                                      .Sum(ms => (ms.ClosingQuantity ?? 0) + ((ms.SubQuantity ?? 0) / 1000m)),

        //                           // ✅ Wastage (assuming all stored in PurchaseUnit already)
        //                           Wastage = rm.WastageDetails
        //                                       .Where(wd => wd.Wastage != null &&
        //                                                    wd.Wastage.Date >= fromDate &&
        //                                                    wd.Wastage.Date <= toDate)
        //                                       .Sum(wd => wd.Quantity ?? 0),

        //                           // ✅ Consumed = Σ(RecipeDetail.Quantity * SaleDetail.Quantity)
        //                           Consumed = rm.RecipeDetails
        //                                        .SelectMany(rd => rd.Recipe.Item.SaleDetails
        //                                            .Where(sd => sd.Sale != null &&
        //                                                         sd.Sale.Date >= fromDate &&
        //                                                         sd.Sale.Date <= toDate)
        //                                            .Select(sd => (rd.Quantity ?? 0) * (sd.Quantity ?? 0))
        //                                        )
        //                                        .Sum(),

        //                           NormalLoss = 0,
        //                           Transfer = 0,
        //                           Shortage = 0,
        //                           Conversion = 0
        //                       }).ToListAsync();

        //    return Ok(query);
        //}






        //[HttpGet("StockSummaryReport")]
        //public async Task<IActionResult> StockSummaryReport(DateTime? fromDate = null, DateTime? toDate = null)
        //{
        //    var today = DateTime.Now;
        //    var startOfMonth = new DateTime(today.Year, today.Month, 1);
        //    var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

        //    fromDate ??= startOfMonth;
        //    toDate ??= endOfMonth;

        //    var query = await (from rm in context.RawMaterials
        //                       select new StockSummary
        //                       {
        //                           RawMaterialName = rm.Name + " [" + rm.PurchaseUnit + "]",

        //                           PurchaseUnit = rm.PurchaseUnit,
        //                           ConsumptionUnit = rm.ConsumptionUnit,
        //                           EquivalentUnit = rm.EquivalentUnit,

        //                           //Opening = rm.OpeningStock ?? 0,


        //                           Opening = (rm.OpeningStockDate < fromDate ? (rm.OpeningStock ?? 0) : 0),


        //                           // ✅ Purchases in date range
        //                           Purchase = rm.PurchaseDetails
        //                                        .Where(pd => pd.Purchase != null &&
        //                                                     pd.Purchase.Date >= fromDate &&
        //                                                     pd.Purchase.Date <= toDate)
        //                                        .Sum(pd => pd.Quantity ?? 0),

        //                           // ✅ Excess = ClosingQuantity + SubQuantity
        //                           Excess = rm.ManageStocks
        //                                      .Where(ms => ms.Date >= fromDate && ms.Date <= toDate)
        //                                      .Sum(ms => (ms.ClosingQuantity ?? 0) + ((ms.SubQuantity ?? 0) / 1000m)),

        //                           // ✅ Wastage (raw in consumption unit)
        //                           Wastage = rm.WastageDetails
        //                                       .Where(wd => wd.Wastage != null &&
        //                                                    wd.Wastage.Date >= fromDate &&
        //                                                    wd.Wastage.Date <= toDate)
        //                                       .Sum(wd => wd.Quantity ?? 0),

        //                           // ✅ Consumed (raw in consumption unit)
        //                           Consumed = rm.RecipeDetails
        //                                        .SelectMany(rd => rd.Recipe.Item.SaleDetails
        //                                            .Where(sd => sd.Sale != null &&
        //                                                         sd.Sale.Date >= fromDate &&
        //                                                         sd.Sale.Date <= toDate)
        //                                            .Select(sd => (rd.Quantity ?? 0) * (sd.Quantity ?? 0))
        //                                        )
        //                                        .Sum(),




        //                           YeildPercenatge = 0,
        //                           Transfer = 0,
        //                           Shortage = 0,
        //                           Production = 0
        //                       }).ToListAsync();

        //    // ✅ Return with both raw & formatted values
        //    //var result = query.Select(q => new
        //    //{
        //    //    q.RawMaterialName,
        //    //    q.PurchaseUnit,
        //    //    q.ConsumptionUnit,
        //    //    q.EquivalentUnit,

        //    //    q.Opening,
        //    //    q.Purchase,
        //    //    q.Excess,

        //    //    Consumed = q.Consumed,
        //    //    ConsumedFormatted = q.ConsumedFormatted,

        //    //    Wastage = q.Wastage,
        //    //    WastageFormatted = q.WastageFormatted,

        //    //    q.NormalLoss,
        //    //    q.Transfer,
        //    //    q.Shortage,
        //    //    q.Conversion
        //    //});


        //    var result = query.Select(q => new
        //    {
        //        q.RawMaterialName,
        //        q.PurchaseUnit,
        //        q.ConsumptionUnit,
        //        q.EquivalentUnit,

        //        Opening = string.Format("{0:0.000}", q.Opening),
        //        Purchase = string.Format("{0:0.000}", q.Purchase),
        //        Excess = string.Format("{0:0.000}", q.Excess),

        //        Consumed = q.Consumed,
        //        ConsumedFormatted = q.ConsumedFormatted,

        //        Wastage =  q.Wastage,
        //        WastageFormatted = q.WastageFormatted,

        //        NormalLoss = string.Format("{0:0.000}", q.YeildPercenatge),
        //        Transfer = string.Format("{0:0.000}", q.Transfer),
        //        Shortage = string.Format("{0:0.000}", q.Shortage),
        //        Conversion = string.Format("{0:0.000}", q.Production)
        //    });





        //    return Ok(result);
        //}















        //[HttpGet("StockSummaryReport1")]
        //public async Task<IActionResult> StockSummaryReport(DateTime? fromDate = null,DateTime? toDate = null,int? categoryId = null)   // ✅ new parameter
        //{
        //    var today = DateTime.Now;
        //    var startOfMonth = new DateTime(today.Year, today.Month, 1);
        //    var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

        //    fromDate ??= startOfMonth;
        //    toDate ??= endOfMonth;

        //    var query = context.RawMaterials
        //                       .AsQueryable();

        //    // ✅ Apply category filter if provided
        //    if (categoryId.HasValue)
        //        query = query.Where(rm => rm.CategoryId == categoryId.Value);

        //    var data = await (from rm in query
        //                      select new StockSummary
        //                      {
        //                          RawMaterialName = rm.Name + " [" + rm.PurchaseUnit + "]",

        //                          PurchaseUnit = rm.PurchaseUnit,
        //                          ConsumptionUnit = rm.ConsumptionUnit,
        //                          EquivalentUnit = rm.EquivalentUnit,
        //                          YeildPercentage = rm.YeildPercenatge,

        //                          Opening = (rm.OpeningStockDate < fromDate ? (rm.OpeningStock ?? 0) : 0),

        //                          // ✅ Purchases
        //                          Purchase = rm.PurchaseDetails
        //                                       .Where(pd => pd.Purchase != null &&
        //                                                    pd.Purchase.Date >= fromDate &&
        //                                                    pd.Purchase.Date <= toDate)
        //                                       .Sum(pd => pd.Quantity ?? 0),

        //                          // ✅ Excess
        //                          Excess = rm.ManageStocks
        //                                     .Where(ms => ms.Date >= fromDate && ms.Date <= toDate)
        //                                     .Sum(ms => (ms.ClosingQuantity ?? 0) + ((ms.SubQuantity ?? 0) / 1000m)),

        //                          // ✅ Wastage
        //                          Wastage = rm.WastageDetails
        //                                      .Where(wd => wd.Wastage != null &&
        //                                                   wd.Wastage.Date >= fromDate &&
        //                                                   wd.Wastage.Date <= toDate)
        //                                      .Sum(wd => wd.Quantity ?? 0),

        //                          // ✅ Consumed
        //                          Consumed = rm.RecipeDetails
        //                                       .SelectMany(rd => rd.Recipe.Item.SaleDetails
        //                                           .Where(sd => sd.Sale != null &&
        //                                                        sd.Sale.Date >= fromDate &&
        //                                                        sd.Sale.Date <= toDate)
        //                                           .Select(sd => (rd.Quantity ?? 0) * (sd.Quantity ?? 0))
        //                                       )
        //                                       .Sum(),



                                 

        //                          //  YeildPercenatge = 0,
        //                          Transfer = 0,
        //                          Shortage = 0,
        //                          Production = 0
        //                      }).ToListAsync();




        //    var result = data.Select(q => new
        //    {

        //        q.RawMaterialName,
        //        q.PurchaseUnit,
        //        q.ConsumptionUnit,
        //        q.EquivalentUnit,

        //        Opening = string.Format("{0:0.000}", q.Opening),
        //        Purchase = string.Format("{0:0.000}", q.Purchase),
        //        Excess = string.Format("{0:0.000}", q.Excess),

        //        Consumed = q.Consumed,
        //        ConsumedFormatted = q.ConsumedFormatted,

        //        Wastage = q.Wastage,
        //        WastageFormatted = q.WastageFormatted,

        //        YeildPercenatge = string.Format("{0:0.000}", q.YeildPercenatge),
        //       // UsableQty = (q.PurchaseQty * q.YieldPercentage) / 100,
        //        Transfer = string.Format("{0:0.000}", q.Transfer),
        //        Shortage = string.Format("{0:0.000}", q.Shortage),
        //        Conversion = string.Format("{0:0.000}", q.Production)
        //    });

        //    return Ok(result);
        //}












        [HttpGet("GetAllProduction")]
        public async Task<IActionResult> GetAllProduction()
        {
            var data = await (
                from production in context.Productions
                join rawmaterial in context.RawMaterials on production.RawMaterialId equals rawmaterial.RawMaterialId
                select new
                {
                    production.ProductionId,
                    production.ProductionName,
                    production.Date,
                    production.RawMaterial,
                    ProductionDetail = context.ProductionDetails
                        .Where(rd => rd.ProductionId == production.ProductionId)
                        .Select(rd => new
                        {
                            rd.ProductionDetailsId,
                            rd.RawMaterialId,
                            rd.Quantity,
                            rd.Unit
                        }).ToList()
                }).OrderByDescending(x=>x.Date).ToListAsync();

            return Ok(data);
        }




        [HttpGet("GetAllProduction1")]
        public async Task<IActionResult> GetAllProduction1()
        {
            var data = await context.Productions
                .Select(production => new
                {
                    production.ProductionId,
                    production.ProductionName,
                    production.Date,
                    RawMaterialName = context.RawMaterials
                        .Where(rm => rm.RawMaterialId == production.RawMaterialId)
                        .Select(rm => rm.Name)
                        .FirstOrDefault(),

                    ProductionDetails = production.ProductionDetails
                        .Select(rd => new
                        {
                            rd.ProductionDetailsId,
                            RawMaterialName = context.RawMaterials
                                .Where(rm => rm.RawMaterialId == rd.RawMaterialId)
                                .Select(rm => rm.Name)
                                .FirstOrDefault(),
                            rd.Quantity,
                            rd.Unit
                        }).ToList()
                }).OrderByDescending(x => x.Date).ToListAsync();

            return Ok(data);
        }


        [HttpPost("Production")]
        public async Task<IActionResult> Production([FromBody] Production request)
        {
            if (request == null || request.ProductionDetails == null || !request.ProductionDetails.Any())
            {
                return BadRequest("Production and at least one ProductionDetail is required.");
            }

            var production = new Production
            {
                //ProductionId=request.ProductionId,
                RawMaterialId = request.RawMaterialId,
                ProductionName=request.ProductionName,
                Quantity = request.Quantity,
                Unit = request.Unit,
                Date = request.Date
            };

            context.Productions.Add(production);
            await context.SaveChangesAsync();

            foreach (var detail in request.ProductionDetails)
            {
                var productiondetail = new ProductionDetail
                {
                    ProductionId = production.ProductionId,
                    RawMaterialId = detail.RawMaterialId,
                    Quantity = detail.Quantity,
                    Unit = detail.Unit,

                };

                context.ProductionDetails.Add(productiondetail);
            }

            await context.SaveChangesAsync(); // Save all ProductionDetails

            return Ok(new { message = "Production created successfully", ProductionId = production.ProductionId });
        }


        [HttpGet("GetProductionById/{id}")]
        public async Task<IActionResult> GetProductionById(int id)
        {
            var data = await (
                from production in context.Productions
                join raw in context.RawMaterials on production.RawMaterialId equals raw.RawMaterialId
                where production.ProductionId == id
                select new
                {
                    production.ProductionId,
                    production.ProductionName,
                    production.Date,
                    RawMaterialId = production.RawMaterialId,
                    RawMaterialName = raw.Name, 
                    production.Quantity, 
                    production.Unit,

                    ProductionDetails = context.ProductionDetails
                        .Where(pd => pd.ProductionId == production.ProductionId)
                        .Select(pd => new
                        {
                            pd.ProductionDetailsId,
                            pd.RawMaterialId,
                            RawMaterialName = context.RawMaterials
                                .Where(r => r.RawMaterialId == pd.RawMaterialId)
                                .Select(r => r.Name)
                                .FirstOrDefault(),
                            pd.Quantity,
                            pd.Unit
                        }).ToList()
                }).FirstOrDefaultAsync();

            if (data == null)
                return NotFound("Production not found.");

            return Ok(data);
        }




        [HttpPut("UpdateProduction/{id}")]
        public async Task<IActionResult> UpdateProduction(int id, [FromBody] Production updatedProduction)
        {
            if (updatedProduction == null || updatedProduction.ProductionDetails == null || !updatedProduction.ProductionDetails.Any())
                return BadRequest("Production and at least one ProductionDetail is required.");

            // Fetch existing Production with details
            var existingProduction = await context.Productions
                .Include(p => p.ProductionDetails)
                .FirstOrDefaultAsync(p => p.ProductionId == id);

            if (existingProduction == null)
                return NotFound("Production not found.");

            // Update main Production fields
            existingProduction.ProductionName = updatedProduction.ProductionName;
            existingProduction.RawMaterialId = updatedProduction.RawMaterialId;
            existingProduction.Quantity = updatedProduction.Quantity;
            existingProduction.Unit = updatedProduction.Unit;
            existingProduction.Date = updatedProduction.Date;

            var existingDetails = existingProduction.ProductionDetails.ToList();
            var incomingDetails = updatedProduction.ProductionDetails.ToList();

            // Remove deleted details
            foreach (var existing in existingDetails)
            {
                if (!incomingDetails.Any(d => d.RawMaterialId == existing.RawMaterialId))
                {
                    context.ProductionDetails.Remove(existing);
                }
            }

            // Add or update details
            foreach (var incoming in incomingDetails)
            {
                var existing = existingDetails.FirstOrDefault(d => d.ProductionDetailsId == incoming.ProductionDetailsId);

                if (existing != null)
                {
                    // Update existing detail
                    existing.RawMaterialId = incoming.RawMaterialId;
                    existing.Quantity = incoming.Quantity;
                    existing.Unit = incoming.Unit;
                }
                else
                {
                    // Add new detail
                    var newDetail = new ProductionDetail
                    {
                        ProductionId = id,
                        RawMaterialId = incoming.RawMaterialId,
                        Quantity = incoming.Quantity,
                        Unit = incoming.Unit
                    };
                    context.ProductionDetails.Add(newDetail);
                }
            }

            await context.SaveChangesAsync();

            return Ok(new { message = "Production updated successfully", ProductionId = existingProduction.ProductionId });
        }



        [HttpDelete("DeleteProduction/{id}")]
        public async Task<IActionResult> DeleteProduction(int id)
        {
            var production = await context.Productions
                .Include(r => r.ProductionDetails) 
                .FirstOrDefaultAsync(r => r.ProductionId == id);

            if (production == null)
                return NotFound("Production not found.");

            context.ProductionDetails.RemoveRange(production.ProductionDetails);

            
            context.Productions.Remove(production);

            await context.SaveChangesAsync();

            return Ok(new { message = "Production deleted successfully", ProductionId = id });
        }



        [HttpGet("GetProductionProcesses")]
        public async Task<IActionResult> GetProductionProcesses()
        {
            var data = await context.Productions
                .Include(p => p.RawMaterial)
                .Select(p => new
                {
                    p.ProductionId,
                    p.ProductionName,
                    p.Quantity,
                    p.Unit,
                    RawMaterialName = p.RawMaterial != null ? p.RawMaterial.Name : ""
                })
                .ToListAsync();

            return Ok(data);
        }



        [HttpGet("GetProductionDetailsByQuantity")]
        public async Task<IActionResult> GetProductionDetailsByQuantity(int productionId, decimal inputQuantity)
        {
            // Get main production record
            var production = await context.Productions
                .FirstOrDefaultAsync(p => p.ProductionId == productionId);

            if (production == null)
                return NotFound("Production not found.");

            // Default base quantity (from master)
            decimal baseQuantity = production.Quantity ?? 1;

            // Fetch production detail ingredients
            var details = await context.ProductionDetails
                .Include(d => d.RawMaterial)
                .Where(d => d.ProductionId == productionId)
                .ToListAsync();

            // Scale quantities according to inputQuantity / baseQuantity
            var scaledDetails = details.Select(d => new
            {
                d.RawMaterialId,
                RawMaterialName = d.RawMaterial != null ? d.RawMaterial.Name : "",
                BaseQuantity = d.Quantity,
                ScaledQuantity = Math.Round((d.Quantity ?? 0) * (inputQuantity / baseQuantity), 3),
                d.Unit
            });

            return Ok(new
            {
                ProductionName = production.ProductionName,
                RawMaterialId = production.RawMaterialId,
                InputQuantity = inputQuantity,
                Unit = production.Unit,
                Ingredients = scaledDetails
            });
        }





        [HttpPost("CreatePurchaseProduction")]
        public async Task<IActionResult> CreatePurchaseProduction([FromBody] Purchase request)
        {
            if (request == null || request.PurchaseDetails == null || !request.PurchaseDetails.Any())
            {
                return BadRequest("Production and at least one ProductionDetail is required.");
            }

            // Auto-generate InvoiceNo
            string newInvoiceNo = "Inv-1";
            var lastSale = await context.Purchases
                .OrderByDescending(s => s.PurchaseId)
                .FirstOrDefaultAsync();

            if (lastSale != null && !string.IsNullOrEmpty(lastSale.InvoiceNo) && lastSale.InvoiceNo.StartsWith("Inv-"))
            {
                var parts = lastSale.InvoiceNo.Split('-');
                if (parts.Length == 2 && int.TryParse(parts[1], out int lastNumber))
                {
                    newInvoiceNo = $"Inv-{lastNumber + 1}";
                }
            }

            var purchase = new Purchase
            {
                InvoiceNo = newInvoiceNo,
                LedgerId = 8051,
                BranchId = request.BranchId,
                UserId = request.UserId,
                Date = request.Date ?? DateTime.Now,
                TotalAmount = 0,
                TotalGst = 0,
                TotalDiscount = 0,
                PaymentStatus = "Paid",
                PaymentMode = "Cash",
                Balance = 0,
                Received = 0
            };

            context.Purchases.Add(purchase);
            await context.SaveChangesAsync();

           

            foreach (var detail in request.PurchaseDetails)
            {
                var purchasedetail = new PurchaseDetail
                {
                    PurchaseId = purchase.PurchaseId, // ✅ correctly assigned
                    RawMaterialId = detail.RawMaterialId,
                    Unit = detail.Unit,
                    Quantity = detail.Quantity,
                    Rate = 0,
                    Amount = 0,
                    DiscountPercentage = 0,
                    DiscountAmount = 0,
                    Gstpercentage = 0,
                    Gstamount = 0,
                    Total = 0
                };

                context.PurchaseDetails.Add(purchasedetail);
            }




            await context.SaveChangesAsync();


            //var totalLedgerBalance = await context.Purchases
            //    .Where(s => s.LedgerId == request.LedgerId)
            //    .SumAsync(s => s.Balance ?? 0);


            //var ledger = await context.Ledgers
            //    .Where(l => l.LedgerId == request.LedgerId)
            //    .FirstOrDefaultAsync();

            //decimal openingBalance = ledger?.OpeningBalance ?? 0;


            //var totalbalance = totalLedgerBalance;


            var transpaurchase = new TransactionDatum
            {
                VoucherNo = Convert.ToString(purchase.PurchaseId),
                Date = request.Date ?? DateTime.Now,
                PayMode = "Cash",
                LedgerId = 8051,
                Total = 0,
                Balance = 0,
                Received = 0,
                TransactionMode = "Purchase",
            };

            context.TransactionData.Add(transpaurchase);
            await context.SaveChangesAsync();




            return Ok(new
            {
                Message = "Production Converted successfully",
                PurchaseId = purchase.PurchaseId,
                InvoiceNo = newInvoiceNo,
             // TotalLedgerBalance = totalLedgerBalance,
             // TotalBalance = totalbalance,


            });
        }






        //[HttpGet("GetProductionDetails/{productionId}")]
        //public async Task<IActionResult> GetProductionDetails(int productionId)
        //{
        //    var details = await context.ProductionDetails
        //        .Include(d => d.RawMaterial)
        //        .Where(d => d.ProductionId == productionId)
        //        .Select(d => new
        //        {
        //            d.ProductionDetailsId,
        //            d.RawMaterialId,
        //            RawMaterialName = d.RawMaterial != null ? d.RawMaterial.Name : "",
        //            d.Quantity,
        //            d.Unit
        //        })
        //        .ToListAsync();

        //    if (details == null || !details.Any())
        //        return NotFound("No details found for this production.");

        //    return Ok(details);
        //}

        //// 👉 3. Add a new production process (for “Send Selected To Production”)
        //[HttpPost("AddProduction")]
        //public async Task<IActionResult> AddProduction([FromBody] Production production)
        //{
        //    if (production == null || production.ProductionDetails == null || !production.ProductionDetails.Any())
        //        return BadRequest("Production and at least one detail are required.");

        //    production.Date = DateTime.Now;
        //    context.Productions.Add(production);
        //    await context.SaveChangesAsync();

        //    return Ok(new { Message = "Production added successfully", production.ProductionId });
        //}




        public static bool IsIdInUse(DbContext context, string idPropertyName, object idValue, Type excludeEntityType)
        {
            var dbSets = context.GetType()
                                .GetProperties()
                                .Where(p => p.PropertyType.IsGenericType &&
                                            p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>));

            foreach (var dbSetProp in dbSets)
            {
                var entityType = dbSetProp.PropertyType.GetGenericArguments().First();

                // Skip the entity/table we want to exclude (like Branches)
                if (entityType == excludeEntityType)
                    continue;

                var property = entityType.GetProperty(idPropertyName);
                if (property == null)
                    continue;

                var dbSet = dbSetProp.GetValue(context);
                var queryable = dbSet as IQueryable;

                var parameter = Expression.Parameter(entityType, "x");
                var propertyAccess = Expression.Property(parameter, property);
                var constant = Expression.Constant(idValue);
                var equality = Expression.Equal(propertyAccess, Expression.Convert(constant, property.PropertyType));
                var lambda = Expression.Lambda(equality, parameter);

                var anyMethod = typeof(Queryable)
                    .GetMethods()
                    .First(m => m.Name == "Any" && m.GetParameters().Length == 2)
                    .MakeGenericMethod(entityType);

                var result = (bool)anyMethod.Invoke(null, new object[] { queryable, lambda });
                if (result)
                    return true;
            }

            return false;
        }



        public static bool IsValueInUse<T>(DbContext context, string propertyName, object value) where T : class
        {
            var property = typeof(T).GetProperty(propertyName);
            if (property == null) return false;

            var parameter = Expression.Parameter(typeof(T), "x");
            var propertyAccess = Expression.Property(parameter, property);
            var constant = Expression.Constant(value);
            var equals = Expression.Equal(propertyAccess, Expression.Convert(constant, property.PropertyType));
            var lambda = Expression.Lambda<Func<T, bool>>(equals, parameter);

            return context.Set<T>().Any(lambda);
        }



    }
}
