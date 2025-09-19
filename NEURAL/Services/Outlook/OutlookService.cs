using NEURAL.Models.Outlook;
using System.Linq;
using System.Xml.Linq;

namespace NEURAL.Services.Outlook
{
    public class OutlookService : IOutlookService
    {
        // Mock data - In real application, this would be Entity Framework DbContext
        private static List<OutlookModel> _outlooks = new List<OutlookModel>
        {
            new OutlookModel
            {
                Id = 1,
                OutlookVersion = "10/06/25 07:00-MBR0.1-OL07-Expert Judgement EC",
                ProdschedVersion = "25/06/25 23:00-MBR0.1",
                ActualVersion = "25/06/25 13:00",
                Status = OutlookStatus.Approved,
                Author = "John Smith",
                CreatedDate = new DateTime(2025, 6, 10),
                Reviewer = "Jane Doe",
                ApprovalDueDate = new DateTime(2025, 6, 15),
                SentDate = new DateTime(2025, 6, 12),
                ApprovedDate = new DateTime(2025, 6, 14),
                Jobsite = "ADARO INDONESIA"
            },
            new OutlookModel
            {
                Id = 2,
                OutlookVersion = "25/06/25 23:00-MBR0.1-OL08-Intervention AI",
                ProdschedVersion = "25/06/25 23:00-MBR0.1",
                ActualVersion = "25/06/25 13:00",
                Status = OutlookStatus.Review,
                Author = "Mike Johnson",
                CreatedDate = new DateTime(2025, 6, 25),
                Reviewer = "Sarah Wilson",
                ApprovalDueDate = new DateTime(2025, 6, 30),
                SentDate = new DateTime(2025, 6, 26),
                Jobsite = "BALANGAN COAL"
            },
            new OutlookModel
            {
                Id = 3,
                ProdschedVersion = "24/06/25 15:00-MBR0.2",
                ActualVersion = "24/06/25 10:00",
                Status = OutlookStatus.Draft,
                Author = "Alex Brown",
                CreatedDate = new DateTime(2025, 6, 24),
                Jobsite = "MARUWAI COAL"
            }
        };

        public OutlookModel CreateOutlook(OutlookModel outlook)
        {
            outlook.Id = _outlooks.Max(o => o.Id) + 1;
            outlook.CreatedDate = DateTime.Now;

            // Generate Outlook Version if status is Review or Approved
            if (outlook.Status == OutlookStatus.Review || outlook.Status == OutlookStatus.Approved)
            {
                outlook.OutlookVersion = GenerateOutlookVersion(outlook.ProdschedVersion, "New Outlook");
            }

            _outlooks.Add(outlook);
            return outlook;
        }

        public OutlookModel? GetOutlookById(int id)
        {
            return _outlooks.FirstOrDefault(o => o.Id == id);
        }

        public OutlookModel UpdateOutlook(OutlookModel outlook)
        {
            var existing = GetOutlookById(outlook.Id);
            if (existing == null)
                throw new ArgumentException("Outlook not found");

            // Update properties
            existing.ProdschedVersion = outlook.ProdschedVersion;
            existing.ActualVersion = outlook.ActualVersion;
            existing.Status = outlook.Status;
            existing.Reviewer = outlook.Reviewer;
            existing.ApprovalDueDate = outlook.ApprovalDueDate;
            existing.SentDate = outlook.SentDate;
            existing.ApprovedDate = outlook.ApprovedDate;
            existing.Jobsite = outlook.Jobsite;

            // Generate Outlook Version if status changed to Review or Approved
            if ((outlook.Status == OutlookStatus.Review || outlook.Status == OutlookStatus.Approved)
                && string.IsNullOrEmpty(existing.OutlookVersion))
            {
                existing.OutlookVersion = GenerateOutlookVersion(outlook.ProdschedVersion, "Updated Outlook");
            }

            return existing;
        }

        public int DeleteOutlooks(int[] ids)
        {
            var deletedCount = 0;
            foreach (var id in ids)
            {
                var outlook = GetOutlookById(id);
                if (outlook != null)
                {
                    _outlooks.Remove(outlook);
                    deletedCount++;
                }
            }
            return deletedCount;
        }

        public KendoDataSourceResultModel GetFilteredOutlookData(KendoDataSourceRequestModel request)
        {
            var query = _outlooks.AsQueryable();

            // Apply filters
            if (request.Filter != null)
            {
                if (!string.IsNullOrEmpty(request.Filter.ProdschedVersion) && request.Filter.ProdschedVersion != "all")
                {
                    query = query.Where(o => o.ProdschedVersion == request.Filter.ProdschedVersion);
                }

                if (!string.IsNullOrEmpty(request.Filter.ActualVersion) && request.Filter.ActualVersion != "all")
                {
                    query = query.Where(o => o.ActualVersion == request.Filter.ActualVersion);
                }

                if (!string.IsNullOrEmpty(request.Filter.Jobsite) && request.Filter.Jobsite != "all")
                {
                    query = query.Where(o => o.Jobsite == request.Filter.Jobsite);
                }
            }

            // Apply search filter
            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                var searchLower = request.SearchTerm.ToLower();
                query = query.Where(o =>
                    (o.OutlookVersion != null && o.OutlookVersion.ToLower().Contains(searchLower)) ||
                    o.Author.ToLower().Contains(searchLower) ||
                    (o.Reviewer != null && o.Reviewer.ToLower().Contains(searchLower))
                );
            }

            // Apply sorting
            if (!string.IsNullOrEmpty(request.SortField))
            {
                switch (request.SortField.ToLower())
                {
                    case "outlookversion":
                        query = request.SortDir == "desc"
                            ? query.OrderByDescending(o => o.OutlookVersion)
                            : query.OrderBy(o => o.OutlookVersion);
                        break;
                    case "prodschedversion":
                        query = request.SortDir == "desc"
                            ? query.OrderByDescending(o => o.ProdschedVersion)
                            : query.OrderBy(o => o.ProdschedVersion);
                        break;
                    case "status":
                        query = request.SortDir == "desc"
                            ? query.OrderByDescending(o => o.Status)
                            : query.OrderBy(o => o.Status);
                        break;
                    case "author":
                        query = request.SortDir == "desc"
                            ? query.OrderByDescending(o => o.Author)
                            : query.OrderBy(o => o.Author);
                        break;
                    case "createddate":
                        query = request.SortDir == "desc"
                            ? query.OrderByDescending(o => o.CreatedDate)
                            : query.OrderBy(o => o.CreatedDate);
                        break;
                    default:
                        query = query.OrderBy(o => o.Id);
                        break;
                }
            }
            else
            {
                query = query.OrderBy(o => o.Id);
            }

            var totalCount = query.Count();

            // Apply paging
            var data = query.Skip(request.Skip).Take(request.Take).ToList();

            return new KendoDataSourceResultModel
            {
                Data = data,
                Total = totalCount
            };
        }

        public List<string> GetProdschedVersionOptions()
        {
            return _outlooks.Select(o => o.ProdschedVersion).Distinct().ToList();
        }

        public List<string> GetActualVersionOptions()
        {
            return _outlooks.Select(o => o.ActualVersion).Distinct().ToList();
        }

        public void UpdateOutlookStatus(int id, OutlookStatus newStatus, string currentUser)
        {
            var outlook = GetOutlookById(id);
            if (outlook == null)
                throw new ArgumentException("Outlook not found");

            var oldStatus = outlook.Status;
            outlook.Status = newStatus;

            // Handle status-specific updates
            switch (newStatus)
            {
                case OutlookStatus.Review:
                    if (string.IsNullOrEmpty(outlook.OutlookVersion))
                    {
                        outlook.OutlookVersion = GenerateOutlookVersion(outlook.ProdschedVersion, "Review Process");
                    }
                    if (!outlook.SentDate.HasValue)
                    {
                        outlook.SentDate = DateTime.Now;
                    }
                    if (string.IsNullOrEmpty(outlook.Reviewer))
                    {
                        outlook.Reviewer = currentUser;
                    }
                    break;

                case OutlookStatus.Approved:
                    outlook.ApprovedDate = DateTime.Now;
                    if (string.IsNullOrEmpty(outlook.OutlookVersion))
                    {
                        outlook.OutlookVersion = GenerateOutlookVersion(outlook.ProdschedVersion, "Approved");
                    }
                    break;

                case OutlookStatus.Draft:
                    // Reset approval-related fields when moving back to draft
                    outlook.ApprovedDate = null;
                    outlook.SentDate = null;
                    break;
            }
        }

        public string GenerateOutlookVersion(string prodschedVersion, string identity)
        {
            var timestamp = DateTime.Now.ToString("dd/MM/yy HH:mm");

            // Extract MBR version from prodsched version
            var mbrVersion = "MBR0.1"; // Default
            if (prodschedVersion.Contains("-MBR"))
            {
                var parts = prodschedVersion.Split("-MBR");
                if (parts.Length > 1)
                {
                    mbrVersion = "MBR" + parts[1];
                }
            }

            // Generate next outlook version number
            var outlookNumber = GetNextOutlookNumber();

            return $"{timestamp}-{mbrVersion}-OL{outlookNumber:D2}-{identity}";
        }

        private int GetNextOutlookNumber()
        {
            var existingNumbers = _outlooks
                .Where(o => !string.IsNullOrEmpty(o.OutlookVersion))
                .Select(o => o.OutlookVersion)
                .Where(v => v.Contains("-OL"))
                .Select(v =>
                {
                    var olIndex = v.IndexOf("-OL") + 3;
                    var nextDash = v.IndexOf("-", olIndex);
                    if (nextDash > olIndex)
                    {
                        var numberStr = v.Substring(olIndex, nextDash - olIndex);
                        if (int.TryParse(numberStr, out int number))
                            return number;
                    }
                    return 0;
                })
                .Where(n => n > 0)
                .ToList();

            return existingNumbers.Any() ? existingNumbers.Max() + 1 : 1;
        }
    }
}
