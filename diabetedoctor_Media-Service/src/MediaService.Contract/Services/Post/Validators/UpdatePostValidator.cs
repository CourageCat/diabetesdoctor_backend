using FluentValidation;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MediaService.Contract.Services.Post.Validators
{
    public class UpdatePostValidator : AbstractValidator<UpdatePostCommand>
    {
        public UpdatePostValidator()
        {
            // Tạo Post
            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Tiêu đề không được để trống!")
                .When(x => !x.IsDraft);
            RuleFor(x => x.Content)
                .NotEmpty()
                .WithMessage("Mô tả không được để trống!")
                .When(x => !x.IsDraft);
            RuleFor(x => x.ContentHtml)
                .NotEmpty()
                .WithMessage("Mô tả dạng html không được để trống!")
                .When(x => !x.IsDraft);
            RuleFor(x => x.Thumbnail)
                .NotEmpty()
                .WithMessage("Ảnh đại diện không được để trống!")
                .Must(thumbnail => ObjectId.TryParse(thumbnail, out _))
                .WithMessage("Ảnh đại diện phải là ObjectId hợp lệ!")
                .When(x => !x.IsDraft);
            RuleFor(x => x.CategoryIds)
                .NotEmpty()
                .WithMessage("Các thể loại của bài viết không được để trống!")
                .Must(categories => categories != null && categories.All(id => ObjectId.TryParse(id, out _)))
                .WithMessage("Danh sách thể loại phải chứa các ObjectId hợp lệ!")
                .When(x => !x.IsDraft);
            RuleFor(x => x.DoctorId)
                .NotEmpty()
                .WithMessage("Id của bác sĩ không được để trống!")
                .When(x => !x.IsDraft);

            RuleFor(x => x.Images)
                .NotNull()
                .WithMessage("Thiếu Id của các hình ảnh!")
                .Must(images => images != null && images.All(id => ObjectId.TryParse(id, out _)))
                .WithMessage("Danh sách hình ảnh phải chứa các ObjectId hợp lệ!")
                .When(x => !x.IsDraft && x.Images != null && x.Images.Count != 0);


            RuleFor(x => x.IsDraft).NotNull().WithMessage("Hãy xác định đang cập nhật bài viết nháp hay tạo mới bài viết!");
        }
    }
}
