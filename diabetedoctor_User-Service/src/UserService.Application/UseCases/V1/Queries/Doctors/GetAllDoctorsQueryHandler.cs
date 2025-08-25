using System.Text;
using UserService.Contract.DTOs.Doctor;
using UserService.Contract.Enums.Doctor;
using UserService.Contract.Services.Doctors.Filters;
using UserService.Contract.Services.Doctors.Queries;
using UserService.Contract.Services.Doctors.Responses;

namespace UserService.Application.UseCases.V1.Queries.Doctors;

public sealed class GetAllDoctorsQueryHandler(ApplicationDbContext context)
    : IQueryHandler<GetAllDoctorsQuery, Success<CursorPagedResult<DoctorResponse>>>
{
    public async Task<Result<Success<CursorPagedResult<DoctorResponse>>>> Handle(GetAllDoctorsQuery query,
        CancellationToken cancellationToken)
    {
        var doctorQuery = context.DoctorProfiles
            .Include(doctor => doctor.User)
            .Include(doctor => doctor.HospitalProfile)
            .AsSplitQuery();
        var cursorValues = new List<string>();
        if (query.Pagination.Cursor != null)
        {
            var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(query.Pagination.Cursor));
            cursorValues.AddRange(decoded.Split('|').ToList());
        }

        doctorQuery = ApplyFilter(doctorQuery, query.Filters, cursorValues);
        doctorQuery = ApplySorting(doctorQuery, query.Filters);
        var doctorFoundAfterPagination = await doctorQuery.Take(query.Pagination.PageSize + 1).ToListAsync(cancellationToken);
        var hasNext = doctorFoundAfterPagination.Count > query.Pagination.PageSize;
        var nextCursor = "";
        if (hasNext)
        {
            doctorFoundAfterPagination.RemoveRange(query.Pagination.PageSize, doctorFoundAfterPagination.Count - query.Pagination.PageSize);
            var lastDoctor = doctorFoundAfterPagination.ToList()[^1];
            nextCursor = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{GetSortCursorValue(lastDoctor, query.Filters.SortBy)}|{lastDoctor.Id}"));
        }
        var doctorProjection = ApplyProjection(doctorFoundAfterPagination).ToList();
        var result = CursorPagedResult<DoctorResponse>.Create(doctorProjection, query.Pagination.PageSize, nextCursor, hasNext);
        return Result.Success(new Success<CursorPagedResult<DoctorResponse>>(
            DoctorMessages.GetAllDoctorsSuccessfully.GetMessage().Code,
            DoctorMessages.GetAllDoctorsSuccessfully.GetMessage().Message,
            result));
    }

    private IQueryable<DoctorProfile> ApplyFilter(IQueryable<DoctorProfile> query, GetAllDoctorsFilter filter, List<string> cursorValues)
    {
        // Always filter out deleted doctors
        query = query.Where(x =>
            x.User.IsDeleted == false
            && x.IsDeleted == false);

        // Filter Cursor
        var isAsc = filter.SortDirection == SortDirectionEnum.Asc;
        if (cursorValues.Count != 0)
        {
            var isParseSecondCursorValueSuccess = Guid.TryParse(cursorValues[1], out Guid idInCursor);
            if (isParseSecondCursorValueSuccess)
            {
                if (filter.SortBy.ToLower().Equals("experiences"))
                {
                    var isParseFirstCursorValueSuccess = int.TryParse(cursorValues[0], out int experienceInCursor);
                    if (isParseFirstCursorValueSuccess)
                    {
                        if (isAsc)
                        {
                            query = query.Where(x =>
                                x.NumberOfExperiences > experienceInCursor ||
                                (x.NumberOfExperiences == experienceInCursor && x.Id > idInCursor));
                        }
                        else
                        {
                            query = query.Where(x =>
                                x.NumberOfExperiences < experienceInCursor ||
                                (x.NumberOfExperiences == experienceInCursor && x.Id < idInCursor));
                        }
                    }
                }
                else if (filter.SortBy.ToLower().Equals("position"))
                {
                    var isParseFirstCursorValueSuccess =
                        Enum.TryParse(cursorValues[0], out DoctorPositionType positionInCursor);
                    if (isParseFirstCursorValueSuccess)
                    {
                        if (isAsc)
                        {
                            query = query.Where(x =>
                                x.Position > positionInCursor ||
                                (x.Position == positionInCursor && x.Id > idInCursor));
                        }
                        else
                        {
                            query = query.Where(x =>
                                x.Position < positionInCursor ||
                                (x.Position == positionInCursor && x.Id < idInCursor));
                        }
                    }
                }
                else if (filter.SortBy.ToLower().Equals("createdDate"))
                {
                    var isParseFirstCursorValueSuccess =
                        DateTime.TryParse(cursorValues[0], out DateTime createdDateInCursor);
                    if (isParseFirstCursorValueSuccess)
                    {
                        if (isAsc)
                        {
                            query = query.Where(x =>
                                x.CreatedDate > createdDateInCursor ||
                                (x.CreatedDate == createdDateInCursor && x.Id > idInCursor));
                        }
                        else
                        {
                            query = query.Where(x =>
                                x.CreatedDate < createdDateInCursor ||
                                (x.CreatedDate == createdDateInCursor && x.Id < idInCursor));
                        }
                    }
                }
                else if (filter.SortBy.ToLower().Equals("dateOfBirth"))
                {
                    var isParseFirstCursorValueSuccess =
                        DateTime.TryParse(cursorValues[0], out DateTime dateOfBirthInCursor);
                    if (isParseFirstCursorValueSuccess)
                    {
                        if (isAsc)
                        {
                            query = query.Where(x =>
                                x.User.DateOfBirth > dateOfBirthInCursor ||
                                (x.User.DateOfBirth == dateOfBirthInCursor && x.Id > idInCursor));
                        }
                        else
                        {
                            query = query.Where(x =>
                                x.User.DateOfBirth < dateOfBirthInCursor ||
                                (x.User.DateOfBirth == dateOfBirthInCursor && x.Id < idInCursor));
                        }
                    }
                }
                else if (filter.SortBy.ToLower().Equals("gender"))
                {
                    var isParseFirstCursorValueSuccess =
                        Enum.TryParse(cursorValues[0], out GenderType genderInCursor);
                    if (isParseFirstCursorValueSuccess)
                    {
                        if (isAsc)
                        {
                            query = query.Where(x =>
                                x.User.Gender > genderInCursor ||
                                (x.User.Gender == genderInCursor && x.Id > idInCursor));
                        }
                        else
                        {
                            query = query.Where(x =>
                                x.User.Gender < genderInCursor ||
                                (x.User.Gender == genderInCursor && x.Id < idInCursor));
                        }
                    }
                }
                else
                {
                    if (isAsc)
                    {
                        query = query.Where(x =>
                            x.User.FullName.LastName.CompareTo(cursorValues[0]) > 0 ||
                            (x.User.FullName.LastName.Equals(cursorValues[0]) && x.Id > idInCursor));
                    }
                    else
                    {
                        query = query.Where(x =>
                            x.User.FullName.LastName.CompareTo(cursorValues[0]) < 0 ||
                            (x.User.FullName.LastName.Equals(cursorValues[0]) && x.Id > idInCursor));
                    }
                }
            }
        }

        // Search by name or introduction
        if (!string.IsNullOrEmpty(filter.Search))
        {
            query = query.Where(x =>
                x.User.DisplayName.ToLower().Contains(filter.Search.ToLower())
                || x.Introduction.Contains(filter.Search));
        }

        // Filter by Gender
        if (filter.Gender != null)
        {
            var genderFilter = filter.Gender.Value.ToEnum<GenderEnum, GenderType>();
            query = query.Where(x => x.User.Gender == genderFilter);
        }

        // Filter by Position
        if (filter.Position != null)
        {
            var position = filter.Position.Value.ToEnum<DoctorPositionEnum, DoctorPositionType>();
            query = query.Where(x => x.Position == position);
        }

        // Filter by HospitalId
        if (filter.HospitalId != null)
        {
            query = query.Where(x => x.HospitalProfileId == filter.HospitalId);
        }

        return query;
    }

    private IQueryable<DoctorProfile> ApplySorting(IQueryable<DoctorProfile> query, GetAllDoctorsFilter filter)
    {
        var isAsc = filter.SortDirection == SortDirectionEnum.Asc;
        query = filter.SortBy switch
        {
            "experiences" => isAsc
                ? query.OrderBy(x => x.NumberOfExperiences).ThenByDescending(x => x.Id)
                : query.OrderByDescending(x => x.NumberOfExperiences).ThenByDescending(x => x.Id),
            "position" => isAsc
                ? query.OrderBy(x => x.Position).ThenByDescending(x => x.Id)
                : query.OrderByDescending(x => x.Position).ThenByDescending(x => x.Id),
            "createdDate" => isAsc
                ? query.OrderBy(x => x.CreatedDate).ThenByDescending(x => x.Id)
                : query.OrderByDescending(x => x.CreatedDate).ThenByDescending(x => x.Id),
            "dateOfBirth" => isAsc 
                ? query.OrderBy(x => x.User.DateOfBirth).ThenByDescending(x => x.Id) 
                : query.OrderByDescending(x => x.User.DateOfBirth).ThenByDescending(x => x.Id),
            "gender" => isAsc 
                ? query.OrderBy(x => x.User.Gender).ThenByDescending(x => x.Id) 
                : query.OrderByDescending(x => x.User.Gender).ThenByDescending(x => x.Id),
            _ => isAsc
                ? query.OrderBy(x => x.User.FullName.LastName).ThenByDescending(x => x.Id)
                : query.OrderByDescending(x => x.User.FullName.LastName).ThenByDescending(x => x.Id),
        };

        return query;
    }

    private IEnumerable<DoctorResponse> ApplyProjection(IEnumerable<DoctorProfile> doctorProfiles)
    {
        return doctorProfiles
            .Select(doctor => new DoctorResponse
            {
                Id = doctor.UserId.ToString(),
                PhoneNumber = doctor.User.PhoneNumber!,
                Avatar = doctor.User.Avatar.Url,
                Name = doctor.User.DisplayName,
                DateOfBirth = doctor.User.DateOfBirth,
                Gender = (GenderEnum)doctor.User.Gender,
                NumberOfExperiences = doctor.NumberOfExperiences,
                Position = (DoctorPositionEnum)doctor.Position,
                CreatedDate = doctor.CreatedDate,
                Hospital = new HospitalDto
                {
                    Id =  doctor.HospitalProfile.Id.ToString(),
                    Name = doctor.HospitalProfile.Name,
                    PhoneNumber = doctor.HospitalProfile.PhoneNumber!,
                    Thumbnail = doctor.HospitalProfile.Thumbnail.Url,
                }
            });
    }
    
    private string GetSortCursorValue(DoctorProfile doctor, string sortBy)
    {
        var result = sortBy switch
        {
            "experiences" => doctor.NumberOfExperiences.ToString(),
            "position" => doctor.Position.ToString(),
            "createdDate" => doctor.CreatedDate.ToString()!,
            "dateOfBirth" => doctor.User.DateOfBirth.ToString(),
            "gender" => doctor.User.Gender.ToString(),
            _ => doctor.User.FullName.LastName,
        };
        return result;
    }
    
    private static Result<Success<CursorPagedResult<DoctorResponse>>> FailureFromMessage(Error error)
    {
        return Result.Failure<Success<CursorPagedResult<DoctorResponse>>>(error);
    }
}