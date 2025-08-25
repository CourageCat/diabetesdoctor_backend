using MediaService.Contract.Enumarations.Post;
using MediaService.Contract.Enumarations.User;
using MediaService.Contract.Helpers;
using MediaService.Domain.Abstractions;
using MediaService.Domain.Enums;
using MediaService.Domain.Models;
using MediaService.Domain.ValueObjects;
using MongoDB.Bson;

namespace MediaService.Persistence.SeedData;

public class SeedData(IMongoDbContext context)
{
    public async Task SeedAsync()
    {
        var moderatorId = ObjectId.GenerateNewId();
        var moderatorGuidId = "83f040ab-1d35-42a1-b081-0df2b660eba6";
        var doctorId = ObjectId.GenerateNewId();
        var doctorGuidId = "9554b171-acdc-42c3-8dec-5d3aba44ca99";
        int numberOfCategories = 11;
        List<ObjectId> listCategoryIds = new List<ObjectId>();
        for (int i = 0; i < numberOfCategories; i++)
        {
            listCategoryIds.Add(ObjectId.GenerateNewId());
        }

        int numberOfProducts = 19;
        List<ObjectId> listPostIds = new List<ObjectId>();
        for (int i = 0; i < numberOfProducts; i++)
        {
            listPostIds.Add(ObjectId.GenerateNewId());
        }

        int numberOfImages = 31;
        List<ObjectId> listImageIds = new List<ObjectId>();
        for (int i = 0; i < numberOfImages; i++)
        {
            listImageIds.Add(ObjectId.GenerateNewId());
        }

        // Seed Data for Category
        bool hasCategoryData = await context.Categories.Find(FilterDefinition<Category>.Empty).AnyAsync();
        if (!hasCategoryData)
        {
            var categories = new List<Category>
            {
                Category.Create(listCategoryIds[0], "Loại 1",
                    "Tiểu đường (đái tháo đường) là một bệnh rối loạn chuyển hóa mạn tính, trong đó cơ thể không sử dụng được glucose do thiếu hụt sản xuất insulin hoặc không sử dụng được insulin hoặc cả hai. Bình thường cơ thể lấy năng lượng từ các thành phần glucose, lipid, protein. Trong đó glucose cung cấp nguồn năng lượng chính cho các tế bào, cho não, cơ…hoạt động. Nhưng muốn sử dụng được glucose thì cần phải có insulin. Insulin là một hormone do tuyến tụy nội tiết sản xuất ra. Insulin giúp cho đường (glucose) từ máu di chuyển vào tế bào, từ đó chuyển hóa và tạo ra năng lượng. Tiểu đường gồm hai thể chính là tiểu đường tuýp 1 và tiểu đường tuýp 2. Tiểu đường tuýp 1 (trước đây còn gọi là tiểu đường phụ thuộc insulin) là bệnh mà có sự phá hủy tế bào beta của đảo tụy (tế bào tiết insulin), gây ra sự thiếu hụt insulin và phải sử dụng nguồn insulin từ bên ngoài đưa vào cơ thể.",
                    Image.Of("loai_1_bopvvg",
                        "https://res.cloudinary.com/dc4eascme/image/upload/v1749465523/diabetesdoctor/loai_1_bopvvg.png")),
                Category.Create(listCategoryIds[1], "Loại 2",
                    "Bệnh đái tháo đường là bệnh rối loạn chuyển hóa không đồng nhất, có đặc điểm tăng glucose huyết do khiếm khuyết về tiết insulin, về tác động của insulin, hoặc cả hai. Tăng glucose mạn tính trong thời gian dài gây nên những rối loạn chuyển hóa carbohydrate, protide, lipide, gây tổn thương ở nhiều cơ quan khác nhau, đặc biệt ở tim và mạch máu, thận, mắt, thần kinh.",
                    Image.Of("loai_2_lhix73",
                        "https://res.cloudinary.com/dc4eascme/image/upload/v1749465523/diabetesdoctor/loai_2_lhix73.png")),
                Category.Create(listCategoryIds[2], "Loại 3",
                    "Tiểu đường type 3, còn gọi là bệnh tiểu đường não vừa xảy ra tổn thương tụy, vừa do viêm mãn tính, vùng não tổn thương là vùng điều hành sản xuất insulin. Như vậy thực tế, tiểu đường type 3 chỉ xảy ra ở người bệnh mắc tiểu đường type 1 hoặc type 2, bệnh nặng hơn và khó điều trị hơn.",
                    Image.Of("loai_3_csxxa7",
                        "https://res.cloudinary.com/dc4eascme/image/upload/v1749465523/diabetesdoctor/loai_3_csxxa7.png")),
                Category.Create(listCategoryIds[3], "Thai kỳ",
                    "Tiểu đường thai kỳ là tình trạng bệnh lý xảy ra do sự rối loạn lượng đường trong máu của phụ nữ mang thai. Đây là một bệnh lý phổ biến thường gặp ở các mẹ bầu. Tuy nhiên, bệnh chỉ phát triển trong thời gian thai kỳ và sẽ biến mất sau khi sinh nở. Các nghiên cứu cho biết rằng, khoảng 2% đến 10% phụ nữ mang thai gặp phải tình trạng này.",
                    Image.Of("thai_ki_yf4vtu",
                        "https://res.cloudinary.com/dc4eascme/image/upload/v1749465523/diabetesdoctor/thai_ki_yf4vtu.png")),
                Category.Create(listCategoryIds[4], "Dinh dưỡng",
                    "Chế độ ăn uống cho người tiểu đường đóng vai trò then chốt trong việc kiểm soát đường huyết và ngăn ngừa biến chứng. Nguyên tắc chính là ăn uống khoa học, cân bằng giữa các nhóm chất và lựa chọn thực phẩm lành mạnh. Người bệnh nên ưu tiên thực phẩm có chỉ số đường huyết thấp như rau xanh, ngũ cốc nguyên hạt, đậu và trái cây ít đường. Cần hạn chế đường, bánh kẹo, nước ngọt và tinh bột tinh chế như cơm trắng, bánh mì trắng. Ngoài ra, nên ăn đủ đạm từ cá, thịt nạc, đậu hũ và chất béo tốt từ dầu ô liu, các loại hạt. Việc chia nhỏ bữa ăn, ăn đúng giờ và kiểm soát khẩu phần sẽ giúp ổn định lượng đường trong máu hiệu quả.",
                    Image.Of("dinh_duong_errluj",
                        "https://res.cloudinary.com/dc4eascme/image/upload/v1749465527/diabetesdoctor/dinh_duong_errluj.png")),
                Category.Create(listCategoryIds[5], "Tâm lí",
                    "Tâm lý của người mắc bệnh tiểu đường thường chịu nhiều ảnh hưởng do việc phải sống chung với bệnh mãn tính. Họ có thể trải qua cảm giác lo lắng, căng thẳng, buồn bã hoặc tự ti vì sự thay đổi trong lối sống, chế độ ăn uống và thói quen sinh hoạt. Một số người có thể thấy áp lực khi phải kiểm soát đường huyết liên tục, sợ biến chứng xảy ra, hoặc cảm thấy bị cô lập khi không thể ăn uống, sinh hoạt như người bình thường. Nếu không được hỗ trợ đúng cách, người bệnh dễ rơi vào tình trạng trầm cảm hoặc suy giảm động lực điều trị. Do đó, bên cạnh chăm sóc y tế, sự đồng hành và chia sẻ từ gia đình, bạn bè và chuyên gia tâm lý là rất cần thiết để giúp họ ổn định tinh thần và sống tích cực hơn.",
                    Image.Of("tam_li_wwxlij",
                        "https://res.cloudinary.com/dc4eascme/image/upload/v1749465523/diabetesdoctor/tam_li_wwxlij.png")),
                Category.Create(listCategoryIds[6], "Thói quen",
                    "Thói quen sinh hoạt lành mạnh đóng vai trò quan trọng trong việc kiểm soát bệnh tiểu đường và ngăn ngừa biến chứng. Người tiểu đường nên duy trì các thói quen ăn uống điều độ, vận động thường xuyên, ngủ đủ giấc, giữ tinh thần thoải mái, uống đủ nước, theo dõi đường huyết định kỳ và tuân thủ hướng dẫn điều trị của bác sĩ.",
                    Image.Of("thoi_quen_tjdjup",
                        "https://res.cloudinary.com/dc4eascme/image/upload/v1749465524/diabetesdoctor/thoi_quen_tjdjup.png")),
                Category.Create(listCategoryIds[7], "Phương pháp",
                    "Các phương pháp điều trị bệnh tiểu đường đa dạng và cần được cá nhân hóa theo tình trạng của từng bệnh nhân. Phương pháp điều trị bao gồm sử dụng thuốc hạ đường huyết, tiêm insulin, áp dụng chế độ ăn kiêng khoa học, thực hiện bài tập thể dục phù hợp, theo dõi chỉ số đường huyết thường xuyên, kiểm soát cân nặng và thực hiện các biện pháp phòng ngừa biến chứng dưới sự giám sát của đội ngũ y tế chuyên khoa.",
                    Image.Of("phuong_phap_qcuo7s",
                        "https://res.cloudinary.com/dc4eascme/image/upload/v1749465523/diabetesdoctor/phuong_phap_qcuo7s.png")),
                Category.Create(listCategoryIds[8], "Triệu chứng",
                    "Triệu chứng bệnh tiểu đường thường xuất hiện từ từ và có thể dễ bị bỏ qua ở giai đoạn đầu. Các dấu hiệu phổ biến bao gồm khát nước nhiều, đi tiểu thường xuyên, đói bất thường, sụt cân không rõ nguyên nhân, mệt mỏi kéo dài, mờ mắt, vết thương lâu lành, hay bị nhiễm trùng, tê bì chân tay và cảm giác ngứa ran ở da. Việc nhận biết sớm các triệu chứng này giúp chẩn đoán và điều trị kịp thời.",
                    Image.Of("trieu_chung_v7swzt",
                        "https://res.cloudinary.com/dc4eascme/image/upload/v1749465523/diabetesdoctor/trieu_chung_v7swzt.png")),
                Category.Create(listCategoryIds[9], "Thiết bị hỗ trợ",
                    "Các thiết bị hỗ trợ hiện đại giúp người tiểu đường quản lý bệnh hiệu quả hơn và nâng cao chất lượng cuộc sống. Những thiết bị này bao gồm máy đo đường huyết cá nhân, que thử và kim chích máu, bút tiêm insulin, máy theo dõi đường huyết liên tục, ứng dụng di động ghi nhận chỉ số sức khỏe, cân điện tử, máy đo huyết áp và các thiết bị thông minh khác hỗ trợ theo dõi và quản lý bệnh tại nhà.",
                    Image.Of("thiet_bi_ho_tro_sjharj",
                        "https://res.cloudinary.com/dc4eascme/image/upload/v1749465523/diabetesdoctor/thiet_bi_ho_tro_sjharj.png")),
                Category.Create(listCategoryIds[10], "Biến chứng",
                    "Biến chứng của bệnh tiểu đường có thể ảnh hưởng nghiêm trọng đến nhiều cơ quan trong cơ thể nếu không được kiểm soát tốt. Các biến chứng phổ biến bao gồm bệnh lý tim mạch, tổn thương thận dẫn đến suy thận, tổn thương mắt có thể gây mù lòa, tổn thương thần kinh ngoại vi, loét chân tiểu đường, nhiễm trùng tái phát, rối loạn chức năng tình dục và các vấn đề về da. Phòng ngừa biến chứng đòi hỏi sự kiểm soát đường huyết nghiêm ngặt và theo dõi y tế định kỳ.",
                    Image.Of("bien_chung_z18r3a",
                        "https://res.cloudinary.com/dc4eascme/image/upload/v1749465527/diabetesdoctor/bien_chung_z18r3a.png"))
            };

            await context.Categories.InsertManyAsync(categories);
        }

        // Seed Data for User
        bool hasUserData = await context.Users.Find(FilterDefinition<User>.Empty).AnyAsync();
        if (!hasUserData)
        {
            var users = new List<User>
            {
                User.Create(moderatorId, "Quản trị viên",
                    Image.Of("Quản trị viên_avatar",
                        "https://res.cloudinary.com/dc4eascme/image/upload/v1750172946/diabetesdoctor/professional-service-manager-vector-bicolor-icon-image_1322553-61939_wfhrfn.avif"),
                    UserId.Of(moderatorGuidId), RoleType.Moderator),
                User.Create(doctorId, "Bác sĩ A",
                    Image.Of("Bác sĩ A_avatar",
                        "https://res.cloudinary.com/dc4eascme/image/upload/v1750172946/diabetesdoctor/vector-illustration-doctor-avatar-photo-doctor-fill-out-questionnaire-banner-set-more-doctor-health-medical-icon_469123-417_nvqosc.avif"),
                    UserId.Of(doctorGuidId), RoleType.Doctor)
            };
            await context.Users.InsertManyAsync(users);
        }

        // Seed Data for Post
        bool hasPostData = await context.Posts.Find(FilterDefinition<Post>.Empty).AnyAsync();
        if (!hasPostData)
        {
            var posts = new List<Post>
            {
                Post.CreateForSeedData(listPostIds[0],
                    "Các phương pháp hiệu quả giúp phòng ngừa bệnh tiểu đường loại 1 và nâng cao sức khỏe toàn diện",
                    Normalize.GetNormalizeString(
                        "Các phương pháp hiệu quả giúp phòng ngừa bệnh tiểu đường loại 1 và nâng cao sức khỏe toàn diện"),
                    "Phòng chống tiểu đường loại 1 Tiểu đường loại 1 là một bệnh tự miễn, xảy ra khi hệ miễn dịch tấn công và phá hủy các tế bào beta trong tuyến tụy – nơi sản xuất insulin. Bệnh thường xuất hiện ở trẻ em và người trẻ tuổi, nhưng cũng có thể xảy ra ở bất kỳ độ tuổi nào. Nguyên nhân gây bệnh: - Yếu tố di truyền - Rối loạn hệ miễn dịch - Ảnh hưởng từ môi trường như virus hoặc thực phẩm trong giai đoạn đầu đời Cách phòng chống: Mặc dù tiểu đường loại 1 không thể phòng ngừa hoàn toàn, nhưng bạn có thể giảm thiểu nguy cơ hoặc kiểm soát tốt hơn bệnh thông qua: 1. Chế độ ăn uống lành mạnh, nhiều rau củ quả, hạn chế đường tinh luyện 2. Tập thể dục đều đặn giúp cân bằng đường huyết 3. Khám sức khỏe định kỳ để phát hiện sớm các dấu hiệu bất thường 4. Giữ tâm lý thoải mái, tránh căng thẳng kéo dài Việc hiểu rõ và chủ động trong chăm sóc sức khỏe sẽ giúp bạn và gia đình phòng ngừa cũng như kiểm soát tốt bệnh tiểu đường loại 1.",
                    "<article>  <h1>Phòng chống tiểu đường loại 1</h1> <img src='https://res.cloudinary.com/dc4eascme/image/upload/v1745489194/diabetesdoctor/causes-of-diabetes_uuq6h1.png' alt='Phòng chống tiểu đường loại 1' style='max-width: 100%; height: auto;'>  <p>Tiểu đường loại 1 là một bệnh tự miễn, xảy ra khi hệ miễn dịch tấn công và phá hủy các tế bào beta trong tuyến tụy – nơi sản xuất insulin. Bệnh thường xuất hiện ở trẻ em và người trẻ tuổi, nhưng cũng có thể xảy ra ở bất kỳ độ tuổi nào.</p>  <h2>Nguyên nhân gây bệnh</h2>  <ul>    <li>Yếu tố di truyền</li>    <li>Rối loạn hệ miễn dịch</li>    <li>Ảnh hưởng từ môi trường như virus hoặc thực phẩm trong giai đoạn đầu đời</li>  </ul>  <h2>Cách phòng chống</h2>  <p>Mặc dù tiểu đường loại 1 không thể phòng ngừa hoàn toàn, nhưng bạn có thể giảm thiểu nguy cơ hoặc kiểm soát tốt hơn bệnh thông qua:</p>  <ol>    <li>Chế độ ăn uống lành mạnh, nhiều rau củ quả, hạn chế đường tinh luyện</li>    <li>Tập thể dục đều đặn giúp cân bằng đường huyết</li>    <li>Khám sức khỏe định kỳ để phát hiện sớm các dấu hiệu bất thường</li>    <li>Giữ tâm lý thoải mái, tránh căng thẳng kéo dài</li>  </ol>  <p>Việc hiểu rõ và chủ động trong chăm sóc sức khỏe sẽ giúp bạn và gia đình phòng ngừa cũng như kiểm soát tốt bệnh tiểu đường loại 1.</p> </article>",
                    Image.Of("cach-dung-duong-de-phong-chong-benh-tieu-duong-medihome_ii6u41",
                        "https://res.cloudinary.com/dc4eascme/image/upload/v1745489049/diabetesdoctor/cach-dung-duong-de-phong-chong-benh-tieu-duong-medihome_ii6u41.jpg"),
                    new List<ObjectId> { listImageIds[11], listImageIds[12] }, UserId.Of(moderatorGuidId),
                    UserId.Of(doctorGuidId), null),
                Post.CreateForSeedData(listPostIds[1],
                    "Nhận biết sớm các triệu chứng và dấu hiệu cảnh báo của bệnh tiểu đường loại 1 để chủ động bảo vệ sức khỏe bản thân hiệu quả nhất.",
                    Normalize.GetNormalizeString(
                        "Nhận biết sớm các triệu chứng và dấu hiệu cảnh báo của bệnh tiểu đường loại 1 để chủ động bảo vệ sức khỏe bản thân hiệu quả nhất."),
                    "Các Triệu Chứng Tiểu Đường Loại 1 Tiểu đường loại 1 là một bệnh tự miễn, trong đó hệ miễn dịch tấn công các tế bào sản xuất insulin trong tuyến tụy. Khi không có đủ insulin, đường trong máu tăng cao dẫn đến nhiều triệu chứng nguy hiểm nếu không được phát hiện và điều trị kịp thời. Triệu chứng thường gặp: - Khát nước liên tục: Cảm giác khát dữ dội dù đã uống nhiều nước. - Đi tiểu thường xuyên: Đặc biệt vào ban đêm do lượng đường trong máu cao khiến cơ thể đào thải qua nước tiểu. - Sút cân nhanh chóng: Mặc dù ăn nhiều nhưng cơ thể không sử dụng được glucose nên phải đốt mỡ và cơ làm năng lượng. - Mệt mỏi kéo dài: Thiếu năng lượng do tế bào không hấp thụ được glucose. - Nhìn mờ: Đường trong máu cao có thể ảnh hưởng đến thủy tinh thể của mắt. - Thường xuyên đói: Do cơ thể không có đủ insulin để đưa đường vào tế bào, dẫn đến cảm giác đói liên tục. - Vết thương lâu lành: Do ảnh hưởng của đường huyết cao đến hệ miễn dịch. - Hơi thở có mùi trái cây hoặc mùi lạ: Đây là dấu hiệu nghiêm trọng, cho thấy cơ thể đang sản sinh ketone – cảnh báo nhiễm toan ceton do tiểu đường. Khi nào nên đi khám? Nếu bạn hoặc người thân, đặc biệt là trẻ em hoặc thanh thiếu niên, có nhiều triệu chứng kể trên, hãy đến ngay cơ sở y tế để kiểm tra đường huyết. Tiểu đường loại 1 có thể khởi phát nhanh và nguy hiểm nếu không được điều trị kịp thời.",
                    "<article>  <h1>Các Triệu Chứng Tiểu Đường Loại 1</h1>  <img src='https://res.cloudinary.com/dc4eascme/image/upload/v1745489194/diabetesdoctor/causes-of-diabetes_uuq6h1.png' alt='Triệu chứng tiểu đường loại 1' style='max-width: 100%; height: auto; margin-bottom: 16px;'>  <p><strong>Tiểu đường loại 1</strong> là một bệnh tự miễn, trong đó hệ miễn dịch tấn công các tế bào sản xuất insulin trong tuyến tụy. Khi không có đủ insulin, đường trong máu tăng cao dẫn đến nhiều triệu chứng nguy hiểm nếu không được phát hiện và điều trị kịp thời.</p>  <h2>Triệu chứng thường gặp</h2>  <ul>    <li><strong>Khát nước liên tục:</strong> Cảm giác khát dữ dội dù đã uống nhiều nước.</li>    <li><strong>Đi tiểu thường xuyên:</strong> Đặc biệt vào ban đêm do lượng đường trong máu cao khiến cơ thể đào thải qua nước tiểu.</li>    <li><strong>Sút cân nhanh chóng:</strong> Mặc dù ăn nhiều nhưng cơ thể không sử dụng được glucose nên phải đốt mỡ và cơ làm năng lượng.</li>    <li><strong>Mệt mỏi kéo dài:</strong> Thiếu năng lượng do tế bào không hấp thụ được glucose.</li>    <li><strong>Nhìn mờ:</strong> Đường trong máu cao có thể ảnh hưởng đến thủy tinh thể của mắt.</li>    <li><strong>Thường xuyên đói:</strong> Do cơ thể không có đủ insulin để đưa đường vào tế bào, dẫn đến cảm giác đói liên tục.</li>    <li><strong>Vết thương lâu lành:</strong> Do ảnh hưởng của đường huyết cao đến hệ miễn dịch.</li>    <li><strong>Hơi thở có mùi trái cây hoặc mùi lạ:</strong> Đây là dấu hiệu nghiêm trọng, cho thấy cơ thể đang sản sinh ketone – cảnh báo nhiễm toan ceton do tiểu đường.</li>  </ul>  <h2>Khi nào nên đi khám?</h2>  <p>Nếu bạn hoặc người thân, đặc biệt là trẻ em hoặc thanh thiếu niên, có nhiều triệu chứng kể trên, hãy đến ngay cơ sở y tế để kiểm tra đường huyết. Tiểu đường loại 1 có thể khởi phát nhanh và nguy hiểm nếu không được điều trị kịp thời.</p> </article>",
                    Image.Of("duong-1_qw2cs1",
                        "https://res.cloudinary.com/dc4eascme/image/upload/v1745489204/diabetesdoctor/duong-1_qw2cs1.webp"),
                    new List<ObjectId> { listImageIds[11], listImageIds[13] }, UserId.Of(moderatorGuidId),
                    UserId.Of(doctorGuidId), null),
                Post.CreateForSeedData(listPostIds[2],
                    "Chế độ ăn uống khoa học, cân bằng dinh dưỡng dành cho người mắc bệnh tiểu đường loại 1 nhằm kiểm soát đường huyết và duy trì sức khỏe ổn định lâu dài.",
                    Normalize.GetNormalizeString(
                        "Chế độ ăn uống khoa học, cân bằng dinh dưỡng dành cho người mắc bệnh tiểu đường loại 1 nhằm kiểm soát đường huyết và duy trì sức khỏe ổn định lâu dài."),
                    "Chế Độ Ăn Uống Cho Người Bị Tiểu Đường Loại 1 Tiểu đường loại 1 là một bệnh lý mạn tính, buộc người bệnh phải tiêm insulin suốt đời. Bên cạnh việc điều trị bằng thuốc, chế độ ăn uống hợp lý đóng vai trò vô cùng quan trọng trong việc kiểm soát đường huyết và phòng tránh biến chứng. Nguyên tắc ăn uống cơ bản: 1. Ăn đủ bữa, đúng giờ: Tránh bỏ bữa để phòng ngừa hạ đường huyết, đặc biệt khi đang sử dụng insulin. 2. Hạn chế đường và tinh bột tinh chế: Như bánh ngọt, nước ngọt, gạo trắng, mì gói. 3. Ưu tiên thực phẩm giàu chất xơ: Ngũ cốc nguyên hạt, rau xanh, trái cây ít ngọt giúp làm chậm hấp thụ đường. 4. Chọn chất đạm lành mạnh: Như thịt nạc, cá, đậu phụ, trứng, sữa không đường. 5. Hạn chế chất béo bão hòa và muối: Để giảm nguy cơ tim mạch. 6. Uống đủ nước: Tránh nước có đường, nên chọn nước lọc hoặc nước thảo mộc không đường. Gợi ý thực đơn trong ngày: Bữa sáng: Yến mạch + sữa ít béo không đường + một quả trứng luộc Bữa trưa: Cơm gạo lứt + cá hấp + rau luộc + canh bí đỏ Bữa tối: Cháo yến mạch + thịt gà + salad rau xanh Bữa phụ: Trái cây ít đường (ổi, táo, bưởi), hạt hạnh nhân không muối Lưu ý quan trọng: 1. Kiểm tra đường huyết trước và sau bữa ăn để điều chỉnh khẩu phần ăn và liều insulin phù hợp. 2. Tham khảo ý kiến bác sĩ hoặc chuyên gia dinh dưỡng để xây dựng thực đơn cá nhân hóa. 3. Không nên tự ý kiêng khem quá mức vì có thể gây hạ đường huyết nguy hiểm. Kết luận: Một chế độ ăn uống khoa học, cân bằng và phù hợp với tình trạng sức khỏe sẽ giúp người mắc tiểu đường loại 1 sống khỏe mạnh, kiểm soát tốt đường huyết và tận hưởng cuộc sống như người bình thường.",
                    "<article>  <h1>Chế Độ Ăn Uống Cho Người Bị Tiểu Đường Loại 1</h1>  <img src='https://res.cloudinary.com/dc4eascme/image/upload/v1745489194/diabetesdoctor/causes-of-diabetes_uuq6h1.png' alt='Chế độ ăn uống cho tiểu đường loại 1' style='max-width: 100%; height: auto; margin-bottom: 16px;'>  <p><strong>Tiểu đường loại 1</strong> là một bệnh lý mạn tính, buộc người bệnh phải tiêm insulin suốt đời. Bên cạnh việc điều trị bằng thuốc, <strong>chế độ ăn uống hợp lý đóng vai trò vô cùng quan trọng</strong> trong việc kiểm soát đường huyết và phòng tránh biến chứng.</p>  <h2>Nguyên tắc ăn uống cơ bản</h2>  <ul>    <li><strong>Ăn đủ bữa, đúng giờ:</strong> Tránh bỏ bữa để phòng ngừa hạ đường huyết, đặc biệt khi đang sử dụng insulin.</li>    <li><strong>Hạn chế đường và tinh bột tinh chế:</strong> Như bánh ngọt, nước ngọt, gạo trắng, mì gói.</li>    <li><strong>Ưu tiên thực phẩm giàu chất xơ:</strong> Ngũ cốc nguyên hạt, rau xanh, trái cây ít ngọt giúp làm chậm hấp thụ đường.</li>    <li><strong>Chọn chất đạm lành mạnh:</strong> Như thịt nạc, cá, đậu phụ, trứng, sữa không đường.</li>    <li><strong>Hạn chế chất béo bão hòa và muối:</strong> Để giảm nguy cơ tim mạch.</li>    <li><strong>Uống đủ nước:</strong> Tránh nước có đường, nên chọn nước lọc hoặc nước thảo mộc không đường.</li>  </ul>  <h2>Gợi ý thực đơn trong ngày</h2>  <p><strong>Bữa sáng:</strong> Yến mạch + sữa ít béo không đường + một quả trứng luộc</p>  <p><strong>Bữa trưa:</strong> Cơm gạo lứt + cá hấp + rau luộc + canh bí đỏ</p>  <p><strong>Bữa tối:</strong> Cháo yến mạch + thịt gà + salad rau xanh</p>  <p><strong>Bữa phụ:</strong> Trái cây ít đường (ổi, táo, bưởi), hạt hạnh nhân không muối</p>  <h2>Lưu ý quan trọng</h2>  <ul>    <li><strong>Kiểm tra đường huyết trước và sau bữa ăn</strong> để điều chỉnh khẩu phần ăn và liều insulin phù hợp.</li>    <li><strong>Tham khảo ý kiến bác sĩ hoặc chuyên gia dinh dưỡng</strong> để xây dựng thực đơn cá nhân hóa.</li>    <li><strong>Không nên tự ý kiêng khem quá mức</strong> vì có thể gây hạ đường huyết nguy hiểm.</li>  </ul>  <h2>Kết luận</h2>  <p>Một chế độ ăn uống khoa học, cân bằng và phù hợp với tình trạng sức khỏe sẽ giúp người mắc tiểu đường loại 1 sống khỏe mạnh, kiểm soát tốt đường huyết và tận hưởng cuộc sống như người bình thường.</p> </article>",
                    Image.Of("an-uong-lanh-manh-cho-nguoi-tieu-duong-loai-1",
                        "https://res.cloudinary.com/dc4eascme/image/upload/v1747761203/diabetesdoctor/an-uong-lanh-manh-cho-nguoi-tieu-duong-loai-1.jpg"),
                    new List<ObjectId> { listImageIds[11], listImageIds[14] }, UserId.Of(moderatorGuidId),
                    UserId.Of(doctorGuidId), null),
                Post.CreateForSeedData(listPostIds[3],
                    "Hướng dẫn xây dựng thói quen ngủ nghỉ khoa học, hợp lý giúp người mắc bệnh tiểu đường loại 1 kiểm soát sức khỏe và cải thiện chất lượng cuộc sống.",
                    Normalize.GetNormalizeString(
                        "Hướng dẫn xây dựng thói quen ngủ nghỉ khoa học, hợp lý giúp người mắc bệnh tiểu đường loại 1 kiểm soát sức khỏe và cải thiện chất lượng cuộc sống."),
                    "Hướng Dẫn Ngủ Nghỉ Khoa Học Cho Người Bị Tiểu Đường Loại 1 Đối với bệnh nhân tiểu đường loại 1, bên cạnh việc điều trị bằng insulin và chế độ ăn uống hợp lý, việc nghỉ ngơi và giấc ngủ đóng vai trò quan trọng trong việc kiểm soát đường huyết và giữ gìn sức khỏe tổng thể. Vì sao giấc ngủ lại quan trọng? 1. Giấc ngủ giúp điều hòa hormone insulin và giảm căng thẳng – yếu tố có thể làm tăng đường huyết. 2. Ngủ không đủ giấc dễ khiến cơ thể mệt mỏi, ăn uống thất thường, khó kiểm soát đường huyết. 3. Ngủ đúng giờ và đủ giấc giúp tăng hiệu quả hấp thu insulin và cải thiện hệ miễn dịch. Nguyên tắc ngủ nghỉ dành cho người tiểu đường loại 1: 1. Ngủ đủ 7 – 8 tiếng mỗi đêm: Đảm bảo chất lượng giấc ngủ, tránh thức khuya hoặc ngủ quá muộn. 2. Giữ lịch trình đi ngủ đều đặn: Ngủ và thức dậy đúng giờ, kể cả cuối tuần. 3. Tránh ăn quá no hoặc quá đói trước khi ngủ: Có thể gây tụt hoặc tăng đường huyết về đêm. 4. Tránh caffeine, trà đặc, rượu và thuốc lá vào buổi tối: Chúng làm giảm chất lượng giấc ngủ. 5. Thư giãn trước khi ngủ: Đọc sách, nghe nhạc nhẹ, thiền hoặc tắm nước ấm. 6. Không sử dụng thiết bị điện tử ít nhất 30 phút trước khi ngủ: Ánh sáng xanh ảnh hưởng đến hormone melatonin. 7. Kiểm tra đường huyết trước khi ngủ: Đảm bảo đường huyết ổn định và giảm nguy cơ hạ đường huyết ban đêm. Giải pháp nếu thường xuyên mất ngủ: 1. Xây dựng môi trường ngủ yên tĩnh, thoáng mát và tối. 2. Không ngủ trưa quá lâu (nên giới hạn dưới 30 phút). 3. Trao đổi với bác sĩ nếu bị mất ngủ kéo dài hoặc nghi ngờ rối loạn giấc ngủ. Kết luận: Giấc ngủ chất lượng và chế độ nghỉ ngơi khoa học sẽ giúp người bệnh tiểu đường loại 1 ổn định đường huyết, tăng cường sức đề kháng và phòng ngừa các biến chứng nguy hiểm.",
                    "<article><h1>Hướng Dẫn Ngủ Nghỉ Khoa Học Cho Người Bị Tiểu Đường Loại 1</h1><img src='https://res.cloudinary.com/dc4eascme/image/upload/v1745489194/diabetesdoctor/causes-of-diabetes_uuq6h1.png' alt='Ngủ nghỉ khoa học cho tiểu đường loại 1' style='max-width: 100%; height: auto; margin-bottom: 16px;'><p>Đối với bệnh nhân tiểu đường loại 1, bên cạnh việc điều trị bằng insulin và chế độ ăn uống hợp lý, việc nghỉ ngơi và giấc ngủ <strong>đóng vai trò quan trọng trong việc kiểm soát đường huyết</strong> và giữ gìn sức khỏe tổng thể.</p><h2>Vì sao giấc ngủ lại quan trọng?</h2><ul><li><strong>Giấc ngủ giúp điều hòa hormone insulin</strong> và giảm căng thẳng – yếu tố có thể làm tăng đường huyết.</li><li><strong>Ngủ không đủ giấc</strong> dễ khiến cơ thể mệt mỏi, ăn uống thất thường, khó kiểm soát đường huyết.</li><li><strong>Ngủ đúng giờ và đủ giấc</strong> giúp tăng hiệu quả hấp thu insulin và cải thiện hệ miễn dịch.</li></ul><h2>Nguyên tắc ngủ nghỉ dành cho người tiểu đường loại 1</h2><ul><li><strong>Ngủ đủ 7 – 8 tiếng mỗi đêm:</strong> Đảm bảo chất lượng giấc ngủ, tránh thức khuya hoặc ngủ quá muộn.</li><li><strong>Giữ lịch trình đi ngủ đều đặn:</strong> Ngủ và thức dậy đúng giờ, kể cả cuối tuần.</li><li><strong>Tránh ăn quá no hoặc quá đói trước khi ngủ:</strong> Có thể gây tụt hoặc tăng đường huyết về đêm.</li><li><strong>Tránh caffeine, trà đặc, rượu và thuốc lá vào buổi tối:</strong> Chúng làm giảm chất lượng giấc ngủ.</li><li><strong>Thư giãn trước khi ngủ:</strong> Đọc sách, nghe nhạc nhẹ, thiền hoặc tắm nước ấm.</li><li><strong>Không sử dụng thiết bị điện tử ít nhất 30 phút trước khi ngủ:</strong> Ánh sáng xanh ảnh hưởng đến hormone melatonin.</li><li><strong>Kiểm tra đường huyết trước khi ngủ:</strong> Đảm bảo đường huyết ổn định và giảm nguy cơ hạ đường huyết ban đêm.</li></ul><h2>Giải pháp nếu thường xuyên mất ngủ</h2><ul><li>Xây dựng môi trường ngủ yên tĩnh, thoáng mát và tối.</li><li>Không ngủ trưa quá lâu (nên giới hạn dưới 30 phút).</li><li>Trao đổi với bác sĩ nếu bị mất ngủ kéo dài hoặc nghi ngờ rối loạn giấc ngủ.</li></ul><h2>Kết luận</h2><p>Giấc ngủ chất lượng và chế độ nghỉ ngơi khoa học sẽ giúp người bệnh tiểu đường loại 1 ổn định đường huyết, tăng cường sức đề kháng và phòng ngừa các biến chứng nguy hiểm.</p></article>",
                    Image.Of("nguoi-lon-tuoi-ngu-1741415343607891898848_jzyh12",
                        "https://res.cloudinary.com/dc4eascme/image/upload/v1745489050/diabetesdoctor/nguoi-lon-tuoi-ngu-1741415343607891898848_jzyh12.webp"),
                    new List<ObjectId> { listImageIds[11], listImageIds[15] }, UserId.Of(moderatorGuidId),
                    UserId.Of(doctorGuidId), null),
                //
                Post.CreateForSeedData(listPostIds[4],
                    "Những lầm tưởng về bệnh tiểu đường loại 1 khiến cha mẹ dễ bỏ qua dấu hiệu ban đầu, làm chậm trễ việc điều trị kịp thời.",
                    Normalize.GetNormalizeString(
                        "Những lầm tưởng về bệnh tiểu đường loại 1 khiến cha mẹ dễ bỏ qua dấu hiệu ban đầu, làm chậm trễ việc điều trị kịp thời."),
                    "Nhiều cha mẹ thường hiểu lầm các dấu hiệu sớm của bệnh tiểu đường loại 1 ở trẻ em, dẫn đến việc chậm trễ trong chẩn đoán và điều trị. Một trong những lầm tưởng phổ biến là nghĩ rằng việc trẻ khát nước liên tục chỉ đơn giản là do trời nóng hoặc do trẻ vận động nhiều. Thực tế, đây có thể là dấu hiệu cảnh báo sớm của bệnh tiểu đường loại 1. Tương tự, khi thấy trẻ đi tiểu nhiều, đặc biệt vào ban đêm, nhiều người lại cho rằng đó là do uống nhiều nước hoặc thói quen cá nhân, mà không nghĩ đến khả năng rối loạn chuyển hóa đường. Một số cha mẹ cũng cho rằng việc trẻ sụt cân có thể là do tăng trưởng bình thường, biếng ăn hoặc do hoạt động nhiều. Trên thực tế, sụt cân không rõ nguyên nhân là dấu hiệu nghiêm trọng cần được kiểm tra. Ngoài ra, khi trẻ thường xuyên mệt mỏi, thiếu năng lượng hoặc có biểu hiện thay đổi hành vi như hay cáu gắt, nhiều người nghĩ rằng đó chỉ là biểu hiện của stress học đường hoặc thay đổi tâm lý tuổi mới lớn. Những hiểu lầm này khiến việc phát hiện bệnh tiểu đường loại 1 bị trì hoãn, làm tăng nguy cơ biến chứng. Vì vậy, nếu trẻ có những biểu hiện như khát nước liên tục, đi tiểu nhiều, sụt cân bất thường, mệt mỏi kéo dài và thay đổi hành vi, cha mẹ nên đưa trẻ đi khám và làm xét nghiệm đường huyết kịp thời.",
                    "<article>  <h1>Những lầm tưởng khiến cha mẹ bỏ qua dấu hiệu sớm của bệnh tiểu đường loại 1 ở trẻ em</h1>  <img src='https://res.cloudinary.com/dc4eascme/image/upload/v1745489194/diabetesdoctor/causes-of-diabetes_uuq6h1.png' alt='Lầm tưởng về tiểu đường loại 1 ở trẻ' style='max-width: 100%; height: auto; margin-bottom: 16px;'>  <p>Nhiều cha mẹ thường hiểu lầm các dấu hiệu sớm của bệnh tiểu đường loại 1 ở trẻ em, dẫn đến việc chậm trễ trong chẩn đoán và điều trị.</p>  <p>Một trong những lầm tưởng phổ biến là nghĩ rằng việc trẻ khát nước liên tục chỉ đơn giản là do trời nóng hoặc do trẻ vận động nhiều. Thực tế, đây có thể là dấu hiệu cảnh báo sớm của bệnh tiểu đường loại 1.</p>  <p>Tương tự, khi thấy trẻ đi tiểu nhiều, đặc biệt vào ban đêm, nhiều người lại cho rằng đó là do uống nhiều nước hoặc thói quen cá nhân, mà không nghĩ đến khả năng rối loạn chuyển hóa đường.</p>  <p>Một số cha mẹ cũng cho rằng việc trẻ sụt cân có thể là do tăng trưởng bình thường, biếng ăn hoặc do hoạt động nhiều. Trên thực tế, sụt cân không rõ nguyên nhân là dấu hiệu nghiêm trọng cần được kiểm tra.</p>  <p>Ngoài ra, khi trẻ thường xuyên mệt mỏi, thiếu năng lượng hoặc có biểu hiện thay đổi hành vi như hay cáu gắt, nhiều người nghĩ rằng đó chỉ là biểu hiện của stress học đường hoặc thay đổi tâm lý tuổi mới lớn.</p>  <p>Những hiểu lầm này khiến việc phát hiện bệnh tiểu đường loại 1 bị trì hoãn, làm tăng nguy cơ biến chứng. Vì vậy, nếu trẻ có những biểu hiện như khát nước liên tục, đi tiểu nhiều, sụt cân bất thường, mệt mỏi kéo dài và thay đổi hành vi, cha mẹ nên đưa trẻ đi khám và làm xét nghiệm đường huyết kịp thời.</p></article>",
                    Image.Of("nhung-lam-tuong-thuong-gap-ve-benh-tieu-duong-tuyp-1_toqubh",
                        "https://res.cloudinary.com/dc4eascme/image/upload/v1746635202/diabetesdoctor/nhung-lam-tuong-thuong-gap-ve-benh-tieu-duong-tuyp-1_toqubh.png"),
                    new List<ObjectId> { listImageIds[11], listImageIds[16] }, UserId.Of(moderatorGuidId),
                    UserId.Of(doctorGuidId), null),
                Post.CreateForSeedData(listPostIds[5],
                    "Tập thể dục đều đặn giúp ổn định đường huyết, cải thiện sức khỏe tim mạch và nâng cao chất lượng cuộc sống cho người mắc tiểu đường loại 1.",
                    Normalize.GetNormalizeString(
                        "Tập thể dục đều đặn giúp ổn định đường huyết, cải thiện sức khỏe tim mạch và nâng cao chất lượng cuộc sống cho người mắc tiểu đường loại 1."),
                    "Vai trò của tập thể dục trong kiểm soát tiểu đường loại 1 Tập thể dục không chỉ giúp kiểm soát cân nặng mà còn làm tăng độ nhạy insulin, giúp giảm mức đường huyết. Lưu ý khi tập: 1. Kiểm tra đường huyết trước và sau khi tập 2. Chuẩn bị sẵn đồ ăn nhẹ có đường trong trường hợp hạ đường huyết 3. Không tập quá sức, ưu tiên các bài tập vừa phải như đi bộ, yoga, đạp xe Tham khảo bác sĩ để lên kế hoạch tập luyện phù hợp với thể trạng.",
                    "<article><h1>Vai trò của tập thể dục trong kiểm soát tiểu đường loại 1</h1><img src='https://res.cloudinary.com/dc4eascme/image/upload/v1745489194/diabetesdoctor/causes-of-diabetes_uuq6h1.png' alt='Tập thể dục và tiểu đường loại 1' style='max-width: 100%; height: auto; margin-bottom: 16px;'><p>Tập thể dục không chỉ giúp kiểm soát cân nặng mà còn làm tăng độ nhạy insulin, giúp giảm mức đường huyết.</p><h2>Lưu ý khi tập:</h2><ul><li>Kiểm tra đường huyết trước và sau khi tập</li><li>Chuẩn bị sẵn đồ ăn nhẹ có đường trong trường hợp hạ đường huyết</li><li>Không tập quá sức, ưu tiên các bài tập vừa phải như đi bộ, yoga, đạp xe</li></ul><p>Tham khảo bác sĩ để lên kế hoạch tập luyện phù hợp với thể trạng.</p></article>",
                    Image.Of("images_yq1gfd",
                        "https://res.cloudinary.com/dc4eascme/image/upload/v1746635202/diabetesdoctor/images_yq1gfd.jpg"),
                    new List<ObjectId> { listImageIds[11], listImageIds[17] }, UserId.Of(moderatorGuidId),
                    UserId.Of(doctorGuidId), null),
                Post.CreateForSeedData(listPostIds[6],
                    "Việc tiêm insulin đúng cách là yếu tố sống còn đối với bệnh nhân tiểu đường loại 1, giúp điều chỉnh đường huyết hiệu quả và tránh biến chứng.",
                    Normalize.GetNormalizeString(
                        "Việc tiêm insulin đúng cách là yếu tố sống còn đối với bệnh nhân tiểu đường loại 1, giúp điều chỉnh đường huyết hiệu quả và tránh biến chứng."),
                    "Tiêm insulin đúng cách và những lưu ý cần thiết Insulin giúp đưa đường từ máu vào tế bào, giảm đường huyết. Kỹ thuật tiêm đúng là rất quan trọng. Các bước cơ bản: 1. Rửa tay sạch trước khi tiêm 2. Thay đổi vị trí tiêm thường xuyên (bụng, đùi, cánh tay) 3. Không tiêm vào vùng đang bị viêm hoặc có sẹo 4. Luôn dùng kim mới, không tái sử dụng Hãy nhờ nhân viên y tế hướng dẫn kỹ thuật tiêm trong lần đầu sử dụng.",
                    "<article><h1>Tiêm insulin đúng cách và những lưu ý cần thiết</h1><img src='https://res.cloudinary.com/dc4eascme/image/upload/v1745489194/diabetesdoctor/causes-of-diabetes_uuq6h1.png' alt='Cách tiêm insulin đúng' style='max-width: 100%; height: auto; margin-bottom: 16px;'><p>Insulin giúp đưa đường từ máu vào tế bào, giảm đường huyết. Kỹ thuật tiêm đúng là rất quan trọng.</p><h2>Các bước cơ bản:</h2><ul><li>Rửa tay sạch trước khi tiêm</li><li>Thay đổi vị trí tiêm thường xuyên (bụng, đùi, cánh tay)</li><li>Không tiêm vào vùng đang bị viêm hoặc có sẹo</li><li>Luôn dùng kim mới, không tái sử dụng</li></ul><p>Hãy nhờ nhân viên y tế hướng dẫn kỹ thuật tiêm trong lần đầu sử dụng.</p></article>",
                    Image.Of("20191026_055453_442338_insulin_max_1800x1800_jpg_e59e59c864_y6jxer",
                        "https://res.cloudinary.com/dc4eascme/image/upload/v1746635201/diabetesdoctor/20191026_055453_442338_insulin_max_1800x1800_jpg_e59e59c864_y6jxer.jpg"),
                    new List<ObjectId> { listImageIds[11], listImageIds[18] }, UserId.Of(moderatorGuidId),
                    UserId.Of(doctorGuidId), null),
                Post.CreateForSeedData(listPostIds[7],
                    "Sống chung với tiểu đường loại 1 không chỉ là cuộc chiến về thể chất mà còn là thử thách tinh thần. Hỗ trợ tâm lý là điều không thể thiếu.",
                    Normalize.GetNormalizeString(
                        "Sống chung với tiểu đường loại 1 không chỉ là cuộc chiến về thể chất mà còn là thử thách tinh thần. Hỗ trợ tâm lý là điều không thể thiếu."),
                    "Tâm lý và cảm xúc của người sống chung với tiểu đường loại 1 Người bệnh dễ cảm thấy lo lắng, căng thẳng, và đôi khi trầm cảm do phải kiểm soát bệnh suốt đời. Cách hỗ trợ: 1. Tham gia nhóm hỗ trợ hoặc gặp chuyên gia tâm lý 2. Chia sẻ cảm xúc với người thân 3. Thực hành thiền, yoga hoặc các hoạt động thư giãn Sức khỏe tinh thần tốt góp phần kiểm soát bệnh hiệu quả hơn.",
                    "<article><h1>Tâm lý và cảm xúc của người sống chung với tiểu đường loại 1</h1><img src='https://res.cloudinary.com/dc4eascme/image/upload/v1745489194/diabetesdoctor/causes-of-diabetes_uuq6h1.png' alt='Tâm lý người bệnh tiểu đường' style='max-width: 100%; height: auto; margin-bottom: 16px;'><p>Người bệnh dễ cảm thấy lo lắng, căng thẳng, và đôi khi trầm cảm do phải kiểm soát bệnh suốt đời.</p><h2>Cách hỗ trợ:</h2><ul><li>Tham gia nhóm hỗ trợ hoặc gặp chuyên gia tâm lý</li><li>Chia sẻ cảm xúc với người thân</li><li>Thực hành thiền, yoga hoặc các hoạt động thư giãn</li></ul><p>Sức khỏe tinh thần tốt góp phần kiểm soát bệnh hiệu quả hơn.</p></article>",
                    Image.Of("d32cb714-benh-tieu-duong-song-duoc-bao-nhieu-nam-phu-thuoc-vao-nhieu-yeu-to_o8f862",
                        "https://res.cloudinary.com/dc4eascme/image/upload/v1746635202/diabetesdoctor/d32cb714-benh-tieu-duong-song-duoc-bao-nhieu-nam-phu-thuoc-vao-nhieu-yeu-to_o8f862.jpg"),
                    new List<ObjectId> { listImageIds[11], listImageIds[19] }, UserId.Of(moderatorGuidId),
                    UserId.Of(doctorGuidId), null),
                Post.CreateForSeedData(listPostIds[8],
                    "Tuổi vị thành niên là giai đoạn nhạy cảm, bệnh tiểu đường loại 1 có thể ảnh hưởng đến thể chất, tâm lý và quá trình phát triển của trẻ.",
                    Normalize.GetNormalizeString(
                        "Tuổi vị thành niên là giai đoạn nhạy cảm, bệnh tiểu đường loại 1 có thể ảnh hưởng đến thể chất, tâm lý và quá trình phát triển của trẻ."),
                    "Tác động của tiểu đường loại 1 đến trẻ vị thành niên Trẻ vị thành niên mắc tiểu đường loại 1 phải học cách tự quản lý bệnh giữa lúc thay đổi về thể chất và tâm lý. Ảnh hưởng thường gặp: 1. Rối loạn cảm xúc hoặc tự ti 2. Thay đổi thói quen sinh hoạt và học tập 3. Gia tăng nguy cơ biến chứng nếu kiểm soát kém Gia đình và nhà trường cần phối hợp hỗ trợ để trẻ phát triển toàn diện.",
                    "<article><h1>Tác động của tiểu đường loại 1 đến trẻ vị thành niên</h1><img src='https://res.cloudinary.com/dc4eascme/image/upload/v1745489194/diabetesdoctor/causes-of-diabetes_uuq6h1.png' alt='Tiểu đường loại 1 ở tuổi dậy thì' style='max-width: 100%; height: auto; margin-bottom: 16px;'><p>Trẻ vị thành niên mắc tiểu đường loại 1 phải học cách tự quản lý bệnh giữa lúc thay đổi về thể chất và tâm lý.</p><h2>Ảnh hưởng thường gặp:</h2><ul><li>Rối loạn cảm xúc hoặc tự ti</li><li>Thay đổi thói quen sinh hoạt và học tập</li><li>Gia tăng nguy cơ biến chứng nếu kiểm soát kém</li></ul><p>Gia đình và nhà trường cần phối hợp hỗ trợ để trẻ phát triển toàn diện.</p></article>",
                    Image.Of("tieu-duong-som-o-tre",
                        "https://res.cloudinary.com/dc4eascme/image/upload/v1746635203/diabetesdoctor/tieu-duong-som-o-tre.jpg"),
                    new List<ObjectId> { listImageIds[11], listImageIds[20] }, UserId.Of(moderatorGuidId),
                    UserId.Of(doctorGuidId), null),
                Post.CreateForSeedData(listPostIds[9],
                    "Máy đo đường huyết giúp người bệnh kiểm soát tốt lượng glucose trong máu. Sử dụng đúng cách là chìa khóa để theo dõi bệnh hiệu quả.",
                    Normalize.GetNormalizeString(
                        "Máy đo đường huyết giúp người bệnh kiểm soát tốt lượng glucose trong máu. Sử dụng đúng cách là chìa khóa để theo dõi bệnh hiệu quả."),
                    "Cách sử dụng máy đo đường huyết tại nhà hiệu quả Đo đường huyết tại nhà giúp bạn kiểm soát chỉ số đường và điều chỉnh chế độ điều trị phù hợp. Hướng dẫn cơ bản: 1. Rửa tay sạch trước khi đo 2. Chọn đầu ngón tay để lấy máu 3. Ghi lại kết quả đo và thời điểm đo 4. Hiệu chuẩn máy định kỳ nếu cần Nên đo vào buổi sáng lúc đói và trước các bữa ăn chính để theo dõi xu hướng.",
                    "<article><h1>Cách sử dụng máy đo đường huyết tại nhà hiệu quả</h1><img src='https://res.cloudinary.com/dc4eascme/image/upload/v1745489194/diabetesdoctor/causes-of-diabetes_uuq6h1.png' alt='Sử dụng máy đo đường huyết' style='max-width: 100%; height: auto; margin-bottom: 16px;'><p>Đo đường huyết tại nhà giúp bạn kiểm soát chỉ số đường và điều chỉnh chế độ điều trị phù hợp.</p><h2>Hướng dẫn cơ bản:</h2><ul><li>Rửa tay sạch trước khi đo</li><li>Chọn đầu ngón tay để lấy máu</li><li>Ghi lại kết quả đo và thời điểm đo</li><li>Hiệu chuẩn máy định kỳ nếu cần</li></ul><p>Nên đo vào buổi sáng lúc đói và trước các bữa ăn chính để theo dõi xu hướng.</p></article>\r\n",
                    Image.Of("may-do-duong-huyet",
                        "https://res.cloudinary.com/dc4eascme/image/upload/v1746635203/diabetesdoctor/may-do-duong-huyet.png"),
                    new List<ObjectId> { listImageIds[11], listImageIds[21] }, UserId.Of(moderatorGuidId),
                    UserId.Of(doctorGuidId), null),
                Post.CreateForSeedData(listPostIds[10],
                    "Tiểu đường loại 1 nếu không kiểm soát tốt có thể dẫn đến nhiều biến chứng nghiêm trọng. Phòng ngừa là chìa khóa.",
                    Normalize.GetNormalizeString(
                        "Tiểu đường loại 1 nếu không kiểm soát tốt có thể dẫn đến nhiều biến chứng nghiêm trọng. Phòng ngừa là chìa khóa."),
                    "Các biến chứng thường gặp và cách phòng tránh Tiểu đường loại 1 kéo dài có thể gây tổn thương mắt, thận, thần kinh và tim mạch. Biến chứng phổ biến: 1. Bệnh võng mạc đái tháo đường 2. Suy thận 3. Tổn thương thần kinh ngoại biên 4. Bệnh mạch vành Để phòng tránh: duy trì đường huyết ổn định, khám định kỳ và thực hiện lối sống lành mạnh.",
                    "<article><h1>Các biến chứng thường gặp và cách phòng tránh</h1><img src='https://res.cloudinary.com/dc4eascme/image/upload/v1745489194/diabetesdoctor/causes-of-diabetes_uuq6h1.png' alt='Biến chứng tiểu đường loại 1' style='max-width: 100%; height: auto; margin-bottom: 16px;'><p>Tiểu đường loại 1 kéo dài có thể gây tổn thương mắt, thận, thần kinh và tim mạch.</p><h2>Biến chứng phổ biến:</h2><ul><li>Bệnh võng mạc đái tháo đường</li><li>Suy thận</li><li>Tổn thương thần kinh ngoại biên</li><li>Bệnh mạch vành</li></ul><p>Để phòng tránh: duy trì đường huyết ổn định, khám định kỳ và thực hiện lối sống lành mạnh.</p></article>",
                    Image.Of("4c572687-7d47-4ada-8e86-7dea4be97758_lueuxt",
                        "https://res.cloudinary.com/dc4eascme/image/upload/v1746635206/diabetesdoctor/4c572687-7d47-4ada-8e86-7dea4be97758_lueuxt.png"),
                    new List<ObjectId> { listImageIds[11], listImageIds[22] }, UserId.Of(moderatorGuidId),
                    UserId.Of(doctorGuidId), null),
                Post.CreateForSeedData(listPostIds[11],
                    "So sánh tiểu đường loại 1 và loại 2: Khác biệt và sai lầm phổ biến",
                    Normalize.GetNormalizeString("So sánh tiểu đường loại 1 và loại 2: Khác biệt và sai lầm phổ biến"),
                    "So sánh tiểu đường loại 1 và loại 2: Khác biệt và sai lầm phổ biến Tiểu đường loại 1 thường xuất hiện ở trẻ em và cần insulin suốt đời, trong khi loại 2 chủ yếu ở người lớn và có thể kiểm soát bằng chế độ sống. Khác biệt chính: 1. Nguyên nhân: Loại 1 là do hệ miễn dịch tấn công tế bào beta tụy, loại 2 do đề kháng insulin 2. Điều trị: Loại 1 cần insulin, loại 2 có thể dùng thuốc uống 3. Sai lầm phổ biến: Nhầm lẫn hai loại bệnh và áp dụng sai cách điều trị",
                    "<article><h1>So sánh tiểu đường loại 1 và loại 2: Khác biệt và sai lầm phổ biến</h1><img src='https://res.cloudinary.com/dc4eascme/image/upload/v1745489194/diabetesdoctor/causes-of-diabetes_uuq6h1.png' alt='So sánh tiểu đường loại 1 và loại 2' style='max-width: 100%; height: auto; margin-bottom: 16px;'><p>Tiểu đường loại 1 thường xuất hiện ở trẻ em và cần insulin suốt đời, trong khi loại 2 chủ yếu ở người lớn và có thể kiểm soát bằng chế độ sống.</p><h2>Khác biệt chính:</h2><ul><li><strong>Nguyên nhân:</strong> Loại 1 là do hệ miễn dịch tấn công tế bào beta tụy, loại 2 do đề kháng insulin</li><li><strong>Điều trị:</strong> Loại 1 cần insulin, loại 2 có thể dùng thuốc uống</li><li><strong>Sai lầm phổ biến:</strong> Nhầm lẫn hai loại bệnh và áp dụng sai cách điều trị</li></ul></article>",
                    Image.Of("phan_biet_va_so_sanh_tieu_duong_type_1_va_type_2_2_Cropped_6b6fba11e1_buyk1a",
                        "https://res.cloudinary.com/dc4eascme/image/upload/v1746635202/diabetesdoctor/phan_biet_va_so_sanh_tieu_duong_type_1_va_type_2_2_Cropped_6b6fba11e1_buyk1a.webp"),
                    new List<ObjectId> { listImageIds[11], listImageIds[23] }, UserId.Of(moderatorGuidId),
                    UserId.Of(doctorGuidId), null),
                Post.CreateForSeedData(listPostIds[12],
                    "Các thiết bị hiện đại và app di động đang giúp người bệnh theo dõi đường huyết và dùng insulin chính xác hơn bao giờ hết.",
                    Normalize.GetNormalizeString(
                        "Các thiết bị hiện đại và app di động đang giúp người bệnh theo dõi đường huyết và dùng insulin chính xác hơn bao giờ hết."),
                    "Ứng dụng công nghệ trong quản lý bệnh tiểu đường loại 1 Các công cụ như máy đo đường huyết liên tục (CGM), bút tiêm thông minh và ứng dụng di động giúp quản lý tiểu đường dễ dàng và chính xác hơn. Các giải pháp nổi bật: 1. CGM giúp theo dõi đường huyết theo thời gian thực 2. App nhắc tiêm insulin và theo dõi bữa ăn 3. Thiết bị đeo hỗ trợ vận động và giấc ngủ Áp dụng công nghệ giúp người bệnh chủ động hơn trong điều trị và cải thiện chất lượng sống.",
                    "<article> <h1>Ứng dụng công nghệ trong quản lý bệnh tiểu đường loại 1</h1> <img src='https://res.cloudinary.com/dc4eascme/image/upload/v1745489194/diabetesdoctor/causes-of-diabetes_uuq6h1.png' alt='Công nghệ hỗ trợ người tiểu đường' style='max-width: 100%; height: auto; margin-bottom: 16px;'> <p>Các công cụ như máy đo đường huyết liên tục (CGM), bút tiêm thông minh và ứng dụng di động giúp quản lý tiểu đường dễ dàng và chính xác hơn.</p> <h2>Các giải pháp nổi bật:</h2> <ul> <li>CGM giúp theo dõi đường huyết theo thời gian thực</li> <li>App nhắc tiêm insulin và theo dõi bữa ăn</li> <li>Thiết bị đeo hỗ trợ vận động và giấc ngủ</li> </ul> <p>Áp dụng công nghệ giúp người bệnh chủ động hơn trong điều trị và cải thiện chất lượng sống.</p></article>",
                    Image.Of("nghien-cuu-moi-ve-dai-thao-duong-type-1-3_fkwexz",
                        "https://res.cloudinary.com/dc4eascme/image/upload/v1746635201/diabetesdoctor/nghien-cuu-moi-ve-dai-thao-duong-type-1-3_fkwexz.webp"),
                    new List<ObjectId> { listImageIds[11], listImageIds[24] }, UserId.Of(moderatorGuidId),
                    UserId.Of(doctorGuidId), null),
                //
                Post.CreateForSeedData(listPostIds[13],
                    "Tìm hiểu các triệu chứng phổ biến của bệnh tiểu đường loại 2 để phát hiện sớm, kiểm soát kịp thời và phòng ngừa những biến chứng nguy hiểm cho sức khỏe.",
                    Normalize.GetNormalizeString(
                        "Tìm hiểu các triệu chứng phổ biến của bệnh tiểu đường loại 2 để phát hiện sớm, kiểm soát kịp thời và phòng ngừa những biến chứng nguy hiểm cho sức khỏe"),
                    "Các Triệu Chứng Tiểu Đường Loại 2 Tiểu đường loại 2 là một bệnh mãn tính xảy ra khi cơ thể không sử dụng insulin một cách hiệu quả, dẫn đến lượng đường trong máu tăng cao. Bệnh thường phát triển từ từ và có thể không có triệu chứng rõ ràng trong giai đoạn đầu. Các triệu chứng phổ biến: 1. Khát nước liên tục và khô miệng 2. Đi tiểu thường xuyên, đặc biệt là vào ban đêm 3. Cảm thấy đói quá mức, ngay cả sau khi ăn 4. Thường xuyên cảm thấy mệt mỏi hoặc kiệt sức 5. Giảm cân không rõ nguyên nhân 6. Thị lực mờ hoặc giảm thị lực 7. Vết thương lâu lành 8. Ngứa da, đặc biệt là vùng kín 9. Tê hoặc ngứa ran ở bàn tay và bàn chân Khi nào nên đi khám? Nếu bạn đang gặp phải nhiều triệu chứng kể trên, đặc biệt là khi có người thân trong gia đình từng mắc tiểu đường, bạn nên đến bệnh viện để kiểm tra đường huyết và nhận lời khuyên từ bác sĩ. Phát hiện sớm giúp kiểm soát bệnh hiệu quả và ngăn ngừa biến chứng nguy hiểm.",
                    "<article> <h1>Các Triệu Chứng Tiểu Đường Loại 2</h1> <img src='https://res.cloudinary.com/dc4eascme/image/upload/v1745489194/diabetesdoctor/causes-of-diabetes_uuq6h1.png' alt='Triệu chứng tiểu đường loại 2' style='max-width: 100%; height: auto;'> <p>Tiểu đường loại 2 là một bệnh mãn tính xảy ra khi cơ thể không sử dụng insulin một cách hiệu quả, dẫn đến lượng đường trong máu tăng cao. Bệnh thường phát triển từ từ và có thể không có triệu chứng rõ ràng trong giai đoạn đầu.</p> <h2>Các triệu chứng phổ biến</h2> <ul> <li>Khát nước liên tục và khô miệng</li> <li>Đi tiểu thường xuyên, đặc biệt là vào ban đêm</li> <li>Cảm thấy đói quá mức, ngay cả sau khi ăn</li> <li>Thường xuyên cảm thấy mệt mỏi hoặc kiệt sức</li> <li>Giảm cân không rõ nguyên nhân</li> <li>Thị lực mờ hoặc giảm thị lực</li> <li>Vết thương lâu lành</li> <li>Ngứa da, đặc biệt là vùng kín</li> <li>Tê hoặc ngứa ran ở bàn tay và bàn chân</li> </ul> <h2>Khi nào nên đi khám?</h2> <p>Nếu bạn đang gặp phải nhiều triệu chứng kể trên, đặc biệt là khi có người thân trong gia đình từng mắc tiểu đường, bạn nên đến bệnh viện để kiểm tra đường huyết và nhận lời khuyên từ bác sĩ. Phát hiện sớm giúp kiểm soát bệnh hiệu quả và ngăn ngừa biến chứng nguy hiểm.</p> </article>",
                    Image.Of("td-1_cl0hlx",
                        "https://res.cloudinary.com/dc4eascme/image/upload/v1745489049/diabetesdoctor/td-1_cl0hlx.jpg"),
                    new List<ObjectId> { listImageIds[11], listImageIds[25] }, UserId.Of(moderatorGuidId),
                    UserId.Of(doctorGuidId), null),
                Post.CreateForSeedData(listPostIds[14],
                    "Tìm hiểu các triệu chứng phổ biến của bệnh tiểu đường loại 3 để phát hiện sớm, kiểm soát kịp thời và phòng ngừa những biến chứng nguy hiểm cho sức khỏe.",
                    Normalize.GetNormalizeString(
                        "Tìm hiểu các triệu chứng phổ biến của bệnh tiểu đường loại 3 để phát hiện sớm, kiểm soát kịp thời và phòng ngừa những biến chứng nguy hiểm cho sức khỏe"),
                    "Các Triệu Chứng của Tiểu Đường Loại 3 Tiểu đường loại 3 là thuật ngữ không chính thức, thường được dùng để mô tả mối liên quan giữa tình trạng rối loạn chuyển hóa insulin trong não và bệnh Alzheimer. Các triệu chứng của tiểu đường loại 3 chủ yếu liên quan đến suy giảm chức năng nhận thức và thay đổi hành vi. Các triệu chứng phổ biến: 1. Giảm trí nhớ, khó khăn trong việc ghi nhớ các sự kiện gần đây. 2. Khó khăn trong việc tập trung và giải quyết vấn đề. 3. Rối loạn ngôn ngữ, gặp khó khăn khi diễn đạt suy nghĩ. 4. Thay đổi tâm trạng như dễ cáu gắt, trầm cảm hoặc lo âu. 5. Mất phương hướng, dễ lạc đường ngay cả ở nơi quen thuộc. 6. Thay đổi trong thói quen ngủ, khó ngủ hoặc ngủ quá nhiều. 7. Giảm khả năng tự chăm sóc bản thân trong sinh hoạt hàng ngày. 8. Rối loạn vận động như đi đứng không vững, dễ té ngã. Khi nào nên đi khám? Nếu bạn hoặc người thân có các biểu hiện suy giảm trí nhớ bất thường, thay đổi hành vi không rõ nguyên nhân, hoặc có tiền sử gia đình mắc tiểu đường type 2 hay Alzheimer, nên đi khám sớm để được đánh giá và can thiệp kịp thời. Phát hiện sớm giúp làm chậm quá trình tiến triển của bệnh và cải thiện chất lượng cuộc sống.",
                    "<article> <h1>Các Triệu Chứng của Tiểu Đường Loại 3</h1> <img src='https://res.cloudinary.com/dc4eascme/image/upload/v1745489194/diabetesdoctor/causes-of-diabetes_uuq6h1.png' alt='Triệu chứng tiểu đường loại 3' style='max-width: 100%; height: auto;'> <p>Tiểu đường loại 3 là thuật ngữ không chính thức, thường được dùng để mô tả mối liên quan giữa tình trạng rối loạn chuyển hóa insulin trong não và bệnh Alzheimer. Các triệu chứng của tiểu đường loại 3 chủ yếu liên quan đến suy giảm chức năng nhận thức và thay đổi hành vi.</p> <h2>Các triệu chứng phổ biến</h2> <ul> <li>Giảm trí nhớ, khó khăn trong việc ghi nhớ các sự kiện gần đây.</li> <li>Khó khăn trong việc tập trung và giải quyết vấn đề.</li> <li>Rối loạn ngôn ngữ, gặp khó khăn khi diễn đạt suy nghĩ.</li> <li>Thay đổi tâm trạng như dễ cáu gắt, trầm cảm hoặc lo âu.</li> <li>Mất phương hướng, dễ lạc đường ngay cả ở nơi quen thuộc.</li> <li>Thay đổi trong thói quen ngủ, khó ngủ hoặc ngủ quá nhiều.</li> <li>Giảm khả năng tự chăm sóc bản thân trong sinh hoạt hàng ngày.</li> <li>Rối loạn vận động như đi đứng không vững, dễ té ngã.</li> </ul> <h2>Khi nào nên đi khám?</h2> <p>Nếu bạn hoặc người thân có các biểu hiện suy giảm trí nhớ bất thường, thay đổi hành vi không rõ nguyên nhân, hoặc có tiền sử gia đình mắc tiểu đường type 2 hay Alzheimer, nên đi khám sớm để được đánh giá và can thiệp kịp thời. Phát hiện sớm giúp làm chậm quá trình tiến triển của bệnh và cải thiện chất lượng cuộc sống.</p> </article>",
                    Image.Of("ebb881de4d30b592ae108a3751b773701731940724_k3roni",
                        "https://res.cloudinary.com/dc4eascme/image/upload/v1745743477/diabetesdoctor/ebb881de4d30b592ae108a3751b773701731940724_k3roni.avif"),
                    new List<ObjectId> { listImageIds[11], listImageIds[26] }, UserId.Of(moderatorGuidId),
                    UserId.Of(doctorGuidId), null),
                Post.CreateForSeedData(listPostIds[15],
                    "Tìm hiểu các triệu chứng phổ biến của bệnh tiểu đường thai kỳ để phát hiện sớm, kiểm soát kịp thời và phòng ngừa những biến chứng nguy hiểm cho sức khỏe.",
                    Normalize.GetNormalizeString(
                        "Tìm hiểu các triệu chứng phổ biến của bệnh tiểu đường thai kỳ để phát hiện sớm, kiểm soát kịp thời và phòng ngừa những biến chứng nguy hiểm cho sức khỏe."),
                    "Các Triệu Chứng của Tiểu Đường Thai Kỳ Tiểu đường thai kỳ là tình trạng lượng đường huyết tăng cao trong thai kỳ, thường xảy ra ở giai đoạn giữa hoặc cuối thai kỳ. Bệnh có thể không có dấu hiệu rõ ràng, do đó việc nhận biết sớm các triệu chứng là rất quan trọng để bảo vệ sức khỏe mẹ và bé. Các triệu chứng phổ biến: 1. Khát nước thường xuyên và cảm giác khô miệng. 2. Đi tiểu nhiều lần hơn bình thường, đặc biệt vào ban đêm. 3. Cảm giác đói liên tục dù đã ăn no. 4. Mệt mỏi, thiếu năng lượng. 5. Thị lực mờ hoặc thay đổi thị lực. 6. Buồn nôn, ói mửa nhẹ (có thể nhầm với ốm nghén). 7. Xuất hiện nhiễm trùng thường xuyên, đặc biệt là nhiễm trùng tiểu hoặc da. 8. Vết thương chậm lành hơn bình thường. Khi nào nên đi khám? Nếu bạn gặp một hoặc nhiều triệu chứng kể trên trong thai kỳ, đặc biệt nếu có yếu tố nguy cơ như béo phì, tiền sử gia đình mắc tiểu đường, hoặc từng bị tiểu đường thai kỳ ở lần mang thai trước, hãy liên hệ ngay với bác sĩ để được kiểm tra đường huyết. Chẩn đoán và kiểm soát sớm sẽ giúp mẹ bầu và thai nhi phát triển khỏe mạnh.",
                    "<article> <h1>Các Triệu Chứng của Tiểu Đường Thai Kỳ</h1> <img src='https://res.cloudinary.com/dc4eascme/image/upload/v1745489194/diabetesdoctor/causes-of-diabetes_uuq6h1.png' alt='Triệu chứng tiểu đường thai kỳ' style='max-width: 100%; height: auto;'> <p>Tiểu đường thai kỳ là tình trạng lượng đường huyết tăng cao trong thai kỳ, thường xảy ra ở giai đoạn giữa hoặc cuối thai kỳ. Bệnh có thể không có dấu hiệu rõ ràng, do đó việc nhận biết sớm các triệu chứng là rất quan trọng để bảo vệ sức khỏe mẹ và bé.</p> <h2>Các triệu chứng phổ biến</h2> <ul> <li>Khát nước thường xuyên và cảm giác khô miệng.</li> <li>Đi tiểu nhiều lần hơn bình thường, đặc biệt vào ban đêm.</li> <li>Cảm giác đói liên tục dù đã ăn no.</li> <li>Mệt mỏi, thiếu năng lượng.</li> <li>Thị lực mờ hoặc thay đổi thị lực.</li> <li>Buồn nôn, ói mửa nhẹ (có thể nhầm với ốm nghén).</li> <li>Xuất hiện nhiễm trùng thường xuyên, đặc biệt là nhiễm trùng tiểu hoặc da.</li> <li>Vết thương chậm lành hơn bình thường.</li> </ul> <h2>Khi nào nên đi khám?</h2> <p>Nếu bạn gặp một hoặc nhiều triệu chứng kể trên trong thai kỳ, đặc biệt nếu có yếu tố nguy cơ như béo phì, tiền sử gia đình mắc tiểu đường, hoặc từng bị tiểu đường thai kỳ ở lần mang thai trước, hãy liên hệ ngay với bác sĩ để được kiểm tra đường huyết. Chẩn đoán và kiểm soát sớm sẽ giúp mẹ bầu và thai nhi phát triển khỏe mạnh.</p> </article>",
                    Image.Of("20190422_075348_015111_11_max_1800x1800_png_30495cf9bb_xlukan",
                        "https://res.cloudinary.com/dc4eascme/image/upload/v1745743604/diabetesdoctor/20190422_075348_015111_11_max_1800x1800_png_30495cf9bb_xlukan.png"),
                    new List<ObjectId> { listImageIds[11], listImageIds[27] }, UserId.Of(moderatorGuidId),
                    UserId.Of(doctorGuidId), null),
                
                Post.CreateForSeedData(listPostIds[16], "Hướng dẫn cách đo tiểu đường bằng máy đo đường huyết HbA1c tại nhà đúng cách, kết quả chính xác",
                    Normalize.GetNormalizeString("Hướng dẫn cách đo tiểu đường bằng máy đo đường huyết HbA1c tại nhà đúng cách, kết quả chính xác"),
                    "Cách đo bằng máy đo đường huyết HbA1c Máy đo HbA1c tại nhà là một công cụ giúp theo dõi mức độ kiểm soát đường huyết trong suốt thời gian dài. Trong đó, HbA1c là chỉ số phản ánh mức đường huyết trung bình trong vòng 2-3 tháng qua. Chỉ số này giúp xác định bệnh tiểu đường của bạn có đang được kiểm soát tốt hay không. Các bước thực hiện: Bước 1: Rửa tay bằng xà phòng và nước ấm, sau đó lau khô bằng khăn sạch. Điều này giúp loại bỏ bụi bẩn và vi khuẩn trên tay, giúp kết quả đo chính xác hơn. Bước 2: Lắp que thử vào khe cắm của máy đo HbA1c. Đảm bảo không chạm vào đầu que thử để tránh làm hỏng cảm biến hoặc gây sai lệch kết quả. Bước 3: Gắn kim chích vào bút chích. Điều chỉnh độ sâu của kim (nếu máy cho phép) để lấy một lượng máu nhỏ. Bước 4: Chích nhẹ vào một đầu ngón tay để lấy một giọt máu nhỏ. Bạn có thể xoa nhẹ ngón tay để máu chảy ra dễ dàng. Bước 5: Đưa đầu ngón tay có máu chạm vào đầu que thử cho đến khi que thử hút đủ máu. Một số máy đo sẽ yêu cầu bạn trộn mẫu máu với dung dịch đệm của máy. Sau đó cho hỗn hợp này lên que thử và chờ kết quả. Bước 6: Chờ kết quả của máy trong vài giây. Kết quả HbA1c được hiển thị dưới dạng phần trăm (%). Bước 7: Tháo và bỏ que thử sau khi đã thử đường huyết xong. Chú ý vệ sinh kim chích và các dụng cụ khác một cách an toàn. Cách đọc kết quả: Bình thường: Dưới 5.7%. Tiền tiểu đường: Từ 5.7% đến 6.4%. Tiểu đường: Từ 6.5% trở lên. Lưu ý: Kết quả trên máy chỉ mang tính tham khảo và cần được đánh giá xác nhận bởi bác sĩ chuyên khoa. Mỗi que thử và kim chích chỉ sử dụng một lần. Đo chỉ số HbA1c định kỳ mỗi tháng hoặc theo sự hướng dẫn của bác sĩ.",
                    "<article><h1>Cách đo bằng máy đo đường huyết HbA1c</h1><p>Máy đo HbA1c tại nhà là một công cụ giúp theo dõi mức độ kiểm soát đường huyết trong suốt thời gian dài. Trong đó, HbA1c là chỉ số phản ánh mức đường huyết trung bình trong vòng 2-3 tháng qua. Chỉ số này giúp xác định bệnh tiểu đường của bạn có đang được kiểm soát tốt hay không.</p><p>Các bước thực hiện:</p><p>Bước 1: Rửa tay bằng xà phòng và nước ấm, sau đó lau khô bằng khăn sạch. Điều này giúp loại bỏ bụi bẩn và vi khuẩn trên tay, giúp kết quả đo chính xác hơn.</p><p>Bước 2: Lắp que thử vào khe cắm của máy đo HbA1c. Đảm bảo không chạm vào đầu que thử để tránh làm hỏng cảm biến hoặc gây sai lệch kết quả.</p><p>Bước 3: Gắn kim chích vào bút chích. Điều chỉnh độ sâu của kim (nếu máy cho phép) để lấy một lượng máu nhỏ.</p><p>Bước 4: Chích nhẹ vào một đầu ngón tay để lấy một giọt máu nhỏ. Bạn có thể xoa nhẹ ngón tay để máu chảy ra dễ dàng.</p><p>Bước 5: Đưa đầu ngón tay có máu chạm vào đầu que thử cho đến khi que thử hút đủ máu. Một số máy đo sẽ yêu cầu bạn trộn mẫu máu với dung dịch đệm của máy. Sau đó cho hỗn hợp này lên que thử và chờ kết quả.</p><p>Bước 6: Chờ kết quả của máy trong vài giây. Kết quả HbA1c được hiển thị dưới dạng phần trăm (%).</p><p>Bước 7: Tháo và bỏ que thử sau khi đã thử đường huyết xong. Chú ý vệ sinh kim chích và các dụng cụ khác một cách an toàn.</p><p>Cách đọc kết quả:</p><p>Bình thường: Dưới 5.7%.</p><p>Tiền tiểu đường: Từ 5.7% đến 6.4%.</p><p>Tiểu đường: Từ 6.5% trở lên.</p><p>Lưu ý:</p><p>Kết quả trên máy chỉ mang tính tham khảo và cần được đánh giá xác nhận bởi bác sĩ chuyên khoa.</p><p>Mỗi que thử và kim chích chỉ sử dụng một lần.</p><p>Đo chỉ số HbA1c định kỳ mỗi tháng hoặc theo sự hướng dẫn của bác sĩ.</p></article>",
                    Image.Of("62c07b82-chi-so-7.2-mmoll-cho-thay-nguy-co-mac-benh-tieu-duong-type-2._iylz8s", "https://res.cloudinary.com/dc4eascme/image/upload/v1755438597/diabetesdoctor/62c07b82-chi-so-7.2-mmoll-cho-thay-nguy-co-mac-benh-tieu-duong-type-2._iylz8s.jpg"),
                    new List<ObjectId>{listImageIds[28]}, UserId.Of(moderatorGuidId),
                    UserId.Of(doctorGuidId), TutorialType.HbA1c),
                
                Post.CreateForSeedData(listPostIds[17], "Hướng dẫn cách đo tiểu đường bằng máy đo đường huyết tại nhà đúng cách, kết quả chính xác",
                    Normalize.GetNormalizeString("Hướng dẫn cách đo tiểu đường bằng máy đo đường huyết tại nhà đúng cách, kết quả chính xác"),
"Cách đo tiểu đường bằng máy đo đường huyết Đây là hình thức đo qua mẫu máu lấy từ ngón tay. Bạn cần đo vào hai thời điểm: lúc đói (trước bữa ăn) và 2 giờ sau ăn. Các bước thực hiện: Bước 1: Rửa tay và lau khô bằng khăn sạch. Điều này giúp loại bỏ bụi bẩn và vi khuẩn trên tay, tránh làm sai lệch kết quả đo. Bước 2: Mở nắp máy đo đường huyết và lắp que thử vào khe cắm theo hướng dẫn trên bao bì. Lưu ý, không chạm tay vào phần tiếp xúc của que thử để tránh làm hỏng cảm biến. Bước 3: Gắn kim chích vào bút chích (lancet). Điều chỉnh độ sâu của kim (nếu máy cho phép) để đảm bảo lấy đủ máu nhưng không quá đau. Bước 4: Chọn vị trí chích trên các đầu ngón tay mềm, như ngón tay giữa hoặc ngón tay đeo nhẫn. Dùng bút chích nhẹ đủ sâu vào đầu ngón tay để lấy một giọt máu nhỏ. Bước 5: Nhẹ nhàng ép nhẹ đầu ngón tay để lấy giọt máu. Không bóp mạnh để tránh làm máu chảy không đều. Bước 6: Đưa đầu ngón tay có máu chạm vào đầu que thử cho đến khi que thử hút đủ máu. Đảm bảo không chạm vào các bộ phận khác của que thử để tránh làm kết quả sai lệch. Bước 7: Chờ kết quả trên màn hình hiển thị của máy trong vài giây. Sau đó ghi lại kết quả đo đường huyết (đơn vị mmol/L hoặc mg/dL tùy loại máy). Bước 8: Tháo que thử ra khỏi máy và bỏ vào thùng rác đúng cách sau khi có kết quả. Chú ý vệ sinh kim chích và các dụng cụ khác một cách an toàn. Cách đọc kết quả đường huyết lúc đói (trước bữa ăn): Bình thường: Dưới 5.6 mmol/L (100 mg/dL). Tiền tiểu đường: Từ 5.6 đến 6.9 mmol/L (100 – 125 mg/dL). Tiểu đường: Từ 7.0 mmol/L (126 mg/dL) trở lên. Cách đọc kết quả đường huyết 2 giờ sau ăn: Bình thường: Dưới 7.8 mmol/L (140 mg/dL). Tiền tiểu đường: Từ 7.8 đến 11.0 mmol/L (140 – 199 mg/dL). Tiểu đường: Từ 11.1 mmol/L (200 mg/dL) trở lên. Lưu ý: Kết quả trên máy chỉ mang tính tham khảo và cần được đánh giá xác nhận bởi bác sĩ chuyên khoa. Mỗi que thử và kim chích chỉ sử dụng một lần. Đo chỉ số đường huyết vào các thời điểm cố định. Lưu lại kết quả đo đường huyết hàng ngày và mang theo cho bác sĩ khi khám bệnh.",
                    "<article><h1>Cách đo tiểu đường bằng máy đo đường huyết</h1><p>Đây là hình thức đo qua mẫu máu lấy từ ngón tay. Bạn cần đo vào hai thời điểm: lúc đói (trước bữa ăn) và 2 giờ sau ăn.</p><p>Các bước thực hiện:</p><p>Bước 1: Rửa tay và lau khô bằng khăn sạch. Điều này giúp loại bỏ bụi bẩn và vi khuẩn trên tay, tránh làm sai lệch kết quả đo.</p><p>Bước 2: Mở nắp máy đo đường huyết và lắp que thử vào khe cắm theo hướng dẫn trên bao bì. Lưu ý, không chạm tay vào phần tiếp xúc của que thử để tránh làm hỏng cảm biến.</p><p>Bước 3: Gắn kim chích vào bút chích (lancet). Điều chỉnh độ sâu của kim (nếu máy cho phép) để đảm bảo lấy đủ máu nhưng không quá đau.</p><p>Bước 4: Chọn vị trí chích trên các đầu ngón tay mềm, như ngón tay giữa hoặc ngón tay đeo nhẫn. Dùng bút chích nhẹ đủ sâu vào đầu ngón tay để lấy một giọt máu nhỏ.</p><p>Bước 5: Nhẹ nhàng ép nhẹ đầu ngón tay để lấy giọt máu. Không bóp mạnh để tránh làm máu chảy không đều.</p><p>Bước 6: Đưa đầu ngón tay có máu chạm vào đầu que thử cho đến khi que thử hút đủ máu. Đảm bảo không chạm vào các bộ phận khác của que thử để tránh làm kết quả sai lệch.</p><p>Bước 7: Chờ kết quả trên màn hình hiển thị của máy trong vài giây. Sau đó ghi lại kết quả đo đường huyết (đơn vị mmol/L hoặc mg/dL tùy loại máy).</p><p>Bước 8: Tháo que thử ra khỏi máy và bỏ vào thùng rác đúng cách sau khi có kết quả. Chú ý vệ sinh kim chích và các dụng cụ khác một cách an toàn.</p><p>Cách đọc kết quả đường huyết lúc đói (trước bữa ăn):</p><p>Bình thường: Dưới 5.6 mmol/L (100 mg/dL).</p><p>Tiền tiểu đường: Từ 5.6 đến 6.9 mmol/L (100 – 125 mg/dL).</p><p>Tiểu đường: Từ 7.0 mmol/L (126 mg/dL) trở lên.</p><p>Cách đọc kết quả đường huyết 2 giờ sau ăn:</p><p>Bình thường: Dưới 7.8 mmol/L (140 mg/dL).</p><p>Tiền tiểu đường: Từ 7.8 đến 11.0 mmol/L (140 – 199 mg/dL).</p><p>Tiểu đường: Từ 11.1 mmol/L (200 mg/dL) trở lên.</p><p>Lưu ý:</p><p>Kết quả trên máy chỉ mang tính tham khảo và cần được đánh giá xác nhận bởi bác sĩ chuyên khoa.</p><p>Mỗi que thử và kim chích chỉ sử dụng một lần.</p><p>Đo chỉ số đường huyết vào các thời điểm cố định.</p><p>Lưu lại kết quả đo đường huyết hàng ngày và mang theo cho bác sĩ khi khám bệnh.</p></article>",
                    Image.Of("cd872f3c-tien-tieu-duong-co-the-dan-den-dai-thao-duong-loai-2-neu-khong-duoc-kiem-soat-tot._yfhhps", "https://res.cloudinary.com/dc4eascme/image/upload/v1755438598/diabetesdoctor/cd872f3c-tien-tieu-duong-co-the-dan-den-dai-thao-duong-loai-2-neu-khong-duoc-kiem-soat-tot._yfhhps.jpg"),
                    new List<ObjectId>{listImageIds[29]}, UserId.Of(moderatorGuidId),
                    UserId.Of(doctorGuidId), TutorialType.BloodGlucose),
                
                Post.CreateForSeedData(listPostIds[18], "Hướng dẫn đo huyết áp tại nhà đúng cách, kết quả chính xác",
                    Normalize.GetNormalizeString("Hướng dẫn đo huyết áp tại nhà đúng cách, kết quả chính xác"),
"Bắt đầu quy trình đo huyết áp Dưới đây là cách tự đo huyết áp chính xác: Chọn tư thế ngồi đúng: Ngồi thẳng lưng, chân để phẳng trên sàn, không bắt chéo chân. Giữ tay tự nhiên và đặt cánh tay ở ngang tầm tim. Đặt bàn tay lên một mặt phẳng như bàn hoặc ghế. Đặt băng quấn đúng vị trí: Đảm bảo băng quấn của máy đo huyết áp được đặt quanh bắp tay, cách khuỷu tay khoảng 2-3 cm. Băng quấn không nên quá chặt hoặc quá lỏng. Khởi động máy và bắt đầu đo: Sau khi đảm bảo máy đo huyết áp đã được đặt đúng, bật máy và bắt đầu quy trình đo. Bạn sẽ cảm nhận băng quấn bơm hơi vào và ngừng bơm sau khi đạt được mức áp suất cần thiết. Trong suốt quá trình này, bạn nên giữ im lặng và không cử động. Đọc kết quả: Sau khi máy tự động xả hơi, kết quả huyết áp sẽ hiển thị trên màn hình, bạn nên ghi lại kết quả để thuận lợi cho việc theo dõi. Ngoài ra, bạn nên đo nhiều lần, sau 1-2 phút để đảm bảo kết quả chuẩn xác. Cách đọc chỉ số huyết áp đo được Kết quả đo huyết áp sẽ bao gồm hai chỉ số quan trọng: Huyết áp tâm thu (chỉ số trên): Chỉ số đo áp lực trong động mạch khi tim co bóp và đẩy máu vào cơ thể. Đây là chỉ số quan trọng nhất để xác định huyết áp cao. Huyết áp tâm trương (chỉ số dưới): Chỉ số đo áp lực trong động mạch khi tim nghỉ ngơi giữa các lần đập. Chỉ số này phản ánh sự đàn hồi của động mạch. Để biết được chỉ số huyết áp có bình thường không, bạn có thể tham khảo phân loại huyết áp theo Hiệp hội Tim mạch Hoa Kỳ (AHA) sau đây: Huyết áp tối ưu: Dưới 120/80 mmHg. Huyết áp bình thường: Khoảng 120/80 – 129/84 mmHg. Huyết áp thấp: Dưới 85/60 mmHg. Tiền tăng huyết áp: Khoảng 130/85 – 139/89 mmHg. Tăng huyết áp độ 1: Khoảng 140/90 – 159/99 mmHg. Tăng huyết áp độ 2: Khoảng 160/100 – 179/109 mmHg. Tăng huyết áp độ 3: Khoảng Từ 180/110 mmHg trở lên. Tăng huyết áp tâm thu đơn độc: Từ 140/90 mmHg trở lên (tâm thu cao, tâm trương bình thường). Ví dụ, nếu kết quả đo của bạn là 117/78 mmHg, nghĩa là huyết áp của bạn đang ở mức bình thường với chỉ số huyết áp tâm thu là 117 mmHg và chỉ số tâm trương là 78 mmHg.",
"<article><h1>Bắt đầu quy trình đo huyết áp</h1><p>Dưới đây là cách tự đo huyết áp chính xác:</p><ul><li><b>Chọn tư thế ngồi đúng:</b> Ngồi thẳng lưng, chân để phẳng trên sàn, không bắt chéo chân. Giữ tay tự nhiên và đặt cánh tay ở ngang tầm tim. Đặt bàn tay lên một mặt phẳng như bàn hoặc ghế.</li><li><b>Đặt băng quấn đúng vị trí:</b> Đảm bảo băng quấn của máy đo huyết áp được đặt quanh bắp tay, cách khuỷu tay khoảng 2-3 cm. Băng quấn không nên quá chặt hoặc quá lỏng.</li><li><b>Khởi động máy và bắt đầu đo:</b> Sau khi đảm bảo máy đo huyết áp đã được đặt đúng, bật máy và bắt đầu quy trình đo. Bạn sẽ cảm nhận băng quấn bơm hơi vào và ngừng bơm sau khi đạt được mức áp suất cần thiết. Trong suốt quá trình này, bạn nên giữ im lặng và không cử động.</li><li><b>Đọc kết quả:</b> Sau khi máy tự động xả hơi, kết quả huyết áp sẽ hiển thị trên màn hình, bạn nên ghi lại kết quả để thuận lợi cho việc theo dõi. Ngoài ra, bạn nên đo nhiều lần, sau 1-2 phút để đảm bảo kết quả chuẩn xác.</li></ul><h2>Cách đọc chỉ số huyết áp đo được</h2><p>Kết quả đo huyết áp sẽ bao gồm hai chỉ số quan trọng:</p><ul><li><b>Huyết áp tâm thu (chỉ số trên):</b> Chỉ số đo áp lực trong động mạch khi tim co bóp và đẩy máu vào cơ thể. Đây là chỉ số quan trọng nhất để xác định huyết áp cao.</li><li><b>Huyết áp tâm trương (chỉ số dưới):</b> Chỉ số đo áp lực trong động mạch khi tim nghỉ ngơi giữa các lần đập. Chỉ số này phản ánh sự đàn hồi của động mạch.</li></ul><p>Để biết được chỉ số huyết áp có bình thường không, bạn có thể tham khảo phân loại huyết áp theo Hiệp hội Tim mạch Hoa Kỳ (AHA) sau đây:</p><ul><li><b>Huyết áp tối ưu:</b> Dưới 120/80 mmHg.</li><li><b>Huyết áp bình thường:</b> Khoảng 120/80 – 129/84 mmHg.</li><li><b>Huyết áp thấp:</b> Dưới 85/60 mmHg.</li><li><b>Tiền tăng huyết áp:</b> Khoảng 130/85 – 139/89 mmHg.</li><li><b>Tăng huyết áp độ 1:</b> Khoảng 140/90 – 159/99 mmHg.</li><li><b>Tăng huyết áp độ 2:</b> Khoảng 160/100 – 179/109 mmHg.</li><li><b>Tăng huyết áp độ 3:</b> Khoảng Từ 180/110 mmHg trở lên.</li><li><b>Tăng huyết áp tâm thu đơn độc:</b> Từ 140/90 mmHg trở lên (tâm thu cao, tâm trương bình thường).</li></ul><p>Ví dụ, nếu kết quả đo của bạn là 117/78 mmHg, nghĩa là huyết áp của bạn đang ở mức bình thường với chỉ số huyết áp tâm thu là 117 mmHg và chỉ số tâm trương là 78 mmHg.</p></article>",
                    Image.Of("23ecb305-ket-qua-do-cua-ban-la-11778-mmhg-co-chi-so-tam-thu-la-117-mmhg-va-tam-truong-la-78-mmhg_nv7vln", "https://res.cloudinary.com/dc4eascme/image/upload/v1755439654/diabetesdoctor/23ecb305-ket-qua-do-cua-ban-la-11778-mmhg-co-chi-so-tam-thu-la-117-mmhg-va-tam-truong-la-78-mmhg_nv7vln.jpg"),
                    new List<ObjectId>{listImageIds[30]}, UserId.Of(moderatorGuidId),
                    UserId.Of(doctorGuidId), TutorialType.BloodPressure),
            };
            await context.Posts.InsertManyAsync(posts);
        }

        // Seed Data for PostCategory
        bool hasPostCategoryData = await context.PostCategories.Find(FilterDefinition<PostCategory>.Empty).AnyAsync();
        if (!hasPostCategoryData)
        {
            var postCategories = new List<PostCategory>
            {
                PostCategory.Create(ObjectId.GenerateNewId(), listPostIds[0], listCategoryIds[0]),
                PostCategory.Create(ObjectId.GenerateNewId(), listPostIds[0], listCategoryIds[7]),
                PostCategory.Create(ObjectId.GenerateNewId(), listPostIds[1], listCategoryIds[0]),
                PostCategory.Create(ObjectId.GenerateNewId(), listPostIds[1], listCategoryIds[8]),
                PostCategory.Create(ObjectId.GenerateNewId(), listPostIds[2], listCategoryIds[0]),
                PostCategory.Create(ObjectId.GenerateNewId(), listPostIds[2], listCategoryIds[4]),
                PostCategory.Create(ObjectId.GenerateNewId(), listPostIds[2], listCategoryIds[7]),
                PostCategory.Create(ObjectId.GenerateNewId(), listPostIds[3], listCategoryIds[0]),
                PostCategory.Create(ObjectId.GenerateNewId(), listPostIds[3], listCategoryIds[6]),
                PostCategory.Create(ObjectId.GenerateNewId(), listPostIds[3], listCategoryIds[7]),
                PostCategory.Create(ObjectId.GenerateNewId(), listPostIds[4], listCategoryIds[0]),
                PostCategory.Create(ObjectId.GenerateNewId(), listPostIds[4], listCategoryIds[8]),
                PostCategory.Create(ObjectId.GenerateNewId(), listPostIds[5], listCategoryIds[0]),
                PostCategory.Create(ObjectId.GenerateNewId(), listPostIds[5], listCategoryIds[7]),
                PostCategory.Create(ObjectId.GenerateNewId(), listPostIds[6], listCategoryIds[0]),
                PostCategory.Create(ObjectId.GenerateNewId(), listPostIds[6], listCategoryIds[7]),
                PostCategory.Create(ObjectId.GenerateNewId(), listPostIds[7], listCategoryIds[0]),
                PostCategory.Create(ObjectId.GenerateNewId(), listPostIds[7], listCategoryIds[5]),
                PostCategory.Create(ObjectId.GenerateNewId(), listPostIds[8], listCategoryIds[0]),
                PostCategory.Create(ObjectId.GenerateNewId(), listPostIds[8], listCategoryIds[5]),
                PostCategory.Create(ObjectId.GenerateNewId(), listPostIds[9], listCategoryIds[0]),
                PostCategory.Create(ObjectId.GenerateNewId(), listPostIds[9], listCategoryIds[7]),
                PostCategory.Create(ObjectId.GenerateNewId(), listPostIds[9], listCategoryIds[9]),
                PostCategory.Create(ObjectId.GenerateNewId(), listPostIds[10], listCategoryIds[0]),
                PostCategory.Create(ObjectId.GenerateNewId(), listPostIds[10], listCategoryIds[10]),
                PostCategory.Create(ObjectId.GenerateNewId(), listPostIds[11], listCategoryIds[0]),
                PostCategory.Create(ObjectId.GenerateNewId(), listPostIds[11], listCategoryIds[1]),
                PostCategory.Create(ObjectId.GenerateNewId(), listPostIds[12], listCategoryIds[0]),
                PostCategory.Create(ObjectId.GenerateNewId(), listPostIds[12], listCategoryIds[9]),
                PostCategory.Create(ObjectId.GenerateNewId(), listPostIds[13], listCategoryIds[1]),
                PostCategory.Create(ObjectId.GenerateNewId(), listPostIds[13], listCategoryIds[8]),
                PostCategory.Create(ObjectId.GenerateNewId(), listPostIds[14], listCategoryIds[2]),
                PostCategory.Create(ObjectId.GenerateNewId(), listPostIds[14], listCategoryIds[8]),
                PostCategory.Create(ObjectId.GenerateNewId(), listPostIds[15], listCategoryIds[3]),
                PostCategory.Create(ObjectId.GenerateNewId(), listPostIds[15], listCategoryIds[8]),
                PostCategory.Create(ObjectId.GenerateNewId(), listPostIds[16], listCategoryIds[7]),
                PostCategory.Create(ObjectId.GenerateNewId(), listPostIds[16], listCategoryIds[9]),
                PostCategory.Create(ObjectId.GenerateNewId(), listPostIds[17], listCategoryIds[7]),
                PostCategory.Create(ObjectId.GenerateNewId(), listPostIds[17], listCategoryIds[9]),
                PostCategory.Create(ObjectId.GenerateNewId(), listPostIds[18], listCategoryIds[7]),
                PostCategory.Create(ObjectId.GenerateNewId(), listPostIds[18], listCategoryIds[9]),
            };
            await context.PostCategories.InsertManyAsync(postCategories);
        }

        // Seed Data for Media
        bool hasMediaData = await context.Medias.Find(FilterDefinition<Media>.Empty).AnyAsync();
        if (!hasMediaData)
        {
            var userId = UserId.Of(moderatorGuidId);
            var medias = new List<Media>
            {
                // Category
                Media.CreateForSeedData(listImageIds[0], "loai_1_bopvvg",
                    "https://res.cloudinary.com/dc4eascme/image/upload/v1749465523/diabetesdoctor/loai_1_bopvvg.png",
                    MediaType.Image, userId),
                Media.CreateForSeedData(listImageIds[1], "loai_2_lhix73",
                    "https://res.cloudinary.com/dc4eascme/image/upload/v1749465523/diabetesdoctor/loai_2_lhix73.png",
                    MediaType.Image, userId),
                Media.CreateForSeedData(listImageIds[2], "loai_3_csxxa7",
                    "https://res.cloudinary.com/dc4eascme/image/upload/v1749465523/diabetesdoctor/loai_3_csxxa7.png",
                    MediaType.Image, userId),
                Media.CreateForSeedData(listImageIds[3], "thai_ki_yf4vtu",
                    "https://res.cloudinary.com/dc4eascme/image/upload/v1749465523/diabetesdoctor/thai_ki_yf4vtu.png",
                    MediaType.Image, userId),
                Media.CreateForSeedData(listImageIds[4], "dinh_duong_errluj",
                    "https://res.cloudinary.com/dc4eascme/image/upload/v1749465527/diabetesdoctor/dinh_duong_errluj.png",
                    MediaType.Image, userId),
                Media.CreateForSeedData(listImageIds[5], "tam_li_wwxlij",
                    "https://res.cloudinary.com/dc4eascme/image/upload/v1749465523/diabetesdoctor/tam_li_wwxlij.png",
                    MediaType.Image, userId),
                Media.CreateForSeedData(listImageIds[6], "thoi_quen_tjdjup",
                    "https://res.cloudinary.com/dc4eascme/image/upload/v1749465524/diabetesdoctor/thoi_quen_tjdjup.png",
                    MediaType.Image, userId),
                Media.CreateForSeedData(listImageIds[7], "phuong_phap_qcuo7s",
                    "https://res.cloudinary.com/dc4eascme/image/upload/v1749465523/diabetesdoctor/phuong_phap_qcuo7s.png",
                    MediaType.Image, userId),
                Media.CreateForSeedData(listImageIds[8], "trieu_chung_v7swzt",
                    "https://res.cloudinary.com/dc4eascme/image/upload/v1749465523/diabetesdoctor/trieu_chung_v7swzt.png",
                    MediaType.Image, userId),
                Media.CreateForSeedData(listImageIds[9], "thiet_bi_ho_tro_sjharj",
                    "https://res.cloudinary.com/dc4eascme/image/upload/v1749465523/diabetesdoctor/thiet_bi_ho_tro_sjharj.png",
                    MediaType.Image, userId),
                Media.CreateForSeedData(listImageIds[10], "bien_chung_z18r3a",
                    "https://res.cloudinary.com/dc4eascme/image/upload/v1749465527/diabetesdoctor/bien_chung_z18r3a.png",
                    MediaType.Image, userId),

                // Post
                Media.CreateForSeedData(listImageIds[11], "causes-of-diabetes_uuq6h1",
                    "https://res.cloudinary.com/dc4eascme/image/upload/v1745489194/diabetesdoctor/causes-of-diabetes_uuq6h1.png",
                    MediaType.Image, userId),
                Media.CreateForSeedData(listImageIds[12],
                    "cach-dung-duong-de-phong-chong-benh-tieu-duong-medihome_ii6u41",
                    "https://res.cloudinary.com/dc4eascme/image/upload/v1745489049/diabetesdoctor/cach-dung-duong-de-phong-chong-benh-tieu-duong-medihome_ii6u41.jpg",
                    MediaType.Image, userId),
                Media.CreateForSeedData(listImageIds[13], "duong-1_qw2cs1",
                    "https://res.cloudinary.com/dc4eascme/image/upload/v1745489204/diabetesdoctor/duong-1_qw2cs1.webp",
                    MediaType.Image, userId),
                Media.CreateForSeedData(listImageIds[14], "an-uong-lanh-manh-cho-nguoi-tieu-duong-loai-1",
                    "https://res.cloudinary.com/dc4eascme/image/upload/v1747761203/diabetesdoctor/an-uong-lanh-manh-cho-nguoi-tieu-duong-loai-1.jpg",
                    MediaType.Image, userId),
                Media.CreateForSeedData(listImageIds[15], "nguoi-lon-tuoi-ngu-1741415343607891898848_jzyh12",
                    "https://res.cloudinary.com/dc4eascme/image/upload/v1745489050/diabetesdoctor/nguoi-lon-tuoi-ngu-1741415343607891898848_jzyh12.webp",
                    MediaType.Image, userId),
                Media.CreateForSeedData(listImageIds[16], "nhung-lam-tuong-thuong-gap-ve-benh-tieu-duong-tuyp-1_toqubh",
                    "https://res.cloudinary.com/dc4eascme/image/upload/v1746635202/diabetesdoctor/nhung-lam-tuong-thuong-gap-ve-benh-tieu-duong-tuyp-1_toqubh.png",
                    MediaType.Image, userId),
                Media.CreateForSeedData(listImageIds[17], "images_yq1gfd",
                    "https://res.cloudinary.com/dc4eascme/image/upload/v1746635202/diabetesdoctor/images_yq1gfd.jpg",
                    MediaType.Image, userId),
                Media.CreateForSeedData(listImageIds[18],
                    "20191026_055453_442338_insulin_max_1800x1800_jpg_e59e59c864_y6jxer",
                    "https://res.cloudinary.com/dc4eascme/image/upload/v1746635201/diabetesdoctor/20191026_055453_442338_insulin_max_1800x1800_jpg_e59e59c864_y6jxer.jpg",
                    MediaType.Image, userId),
                Media.CreateForSeedData(listImageIds[19],
                    "d32cb714-benh-tieu-duong-song-duoc-bao-nhieu-nam-phu-thuoc-vao-nhieu-yeu-to_o8f862",
                    "https://res.cloudinary.com/dc4eascme/image/upload/v1746635202/diabetesdoctor/d32cb714-benh-tieu-duong-song-duoc-bao-nhieu-nam-phu-thuoc-vao-nhieu-yeu-to_o8f862.jpg",
                    MediaType.Image, userId),
                Media.CreateForSeedData(listImageIds[20], "tieu-duong-som-o-tre",
                    "https://res.cloudinary.com/dc4eascme/image/upload/v1746635203/diabetesdoctor/tieu-duong-som-o-tre.jpg",
                    MediaType.Image, userId),
                Media.CreateForSeedData(listImageIds[21], "may-do-duong-huyet",
                    "https://res.cloudinary.com/dc4eascme/image/upload/v1746635203/diabetesdoctor/may-do-duong-huyet.png",
                    MediaType.Image, userId),
                Media.CreateForSeedData(listImageIds[22], "4c572687-7d47-4ada-8e86-7dea4be97758_lueuxt",
                    "https://res.cloudinary.com/dc4eascme/image/upload/v1746635206/diabetesdoctor/4c572687-7d47-4ada-8e86-7dea4be97758_lueuxt.png",
                    MediaType.Image, userId),
                Media.CreateForSeedData(listImageIds[23],
                    "phan_biet_va_so_sanh_tieu_duong_type_1_va_type_2_2_Cropped_6b6fba11e1_buyk1a",
                    "https://res.cloudinary.com/dc4eascme/image/upload/v1746635202/diabetesdoctor/phan_biet_va_so_sanh_tieu_duong_type_1_va_type_2_2_Cropped_6b6fba11e1_buyk1a.webp",
                    MediaType.Image, userId),
                Media.CreateForSeedData(listImageIds[24], "nghien-cuu-moi-ve-dai-thao-duong-type-1-3_fkwexz",
                    "https://res.cloudinary.com/dc4eascme/image/upload/v1746635201/diabetesdoctor/nghien-cuu-moi-ve-dai-thao-duong-type-1-3_fkwexz.webp",
                    MediaType.Image, userId),
                Media.CreateForSeedData(listImageIds[25], "td-1_cl0hlx",
                    "https://res.cloudinary.com/dc4eascme/image/upload/v1745489049/diabetesdoctor/td-1_cl0hlx.jpg",
                    MediaType.Image, userId),
                Media.CreateForSeedData(listImageIds[26], "ebb881de4d30b592ae108a3751b773701731940724_k3roni",
                    "https://res.cloudinary.com/dc4eascme/image/upload/v1745743477/diabetesdoctor/ebb881de4d30b592ae108a3751b773701731940724_k3roni.avif",
                    MediaType.Image, userId),
                Media.CreateForSeedData(listImageIds[27],
                    "20190422_075348_015111_11_max_1800x1800_png_30495cf9bb_xlukan",
                    "https://res.cloudinary.com/dc4eascme/image/upload/v1745743604/diabetesdoctor/20190422_075348_015111_11_max_1800x1800_png_30495cf9bb_xlukan.png",
                    MediaType.Image, userId),
                Media.CreateForSeedData(listImageIds[28],
                    "62c07b82-chi-so-7.2-mmoll-cho-thay-nguy-co-mac-benh-tieu-duong-type-2._iylz8s",
                    "https://res.cloudinary.com/dc4eascme/image/upload/v1755438597/diabetesdoctor/62c07b82-chi-so-7.2-mmoll-cho-thay-nguy-co-mac-benh-tieu-duong-type-2._iylz8s.jpg",
                    MediaType.Image, userId),
                Media.CreateForSeedData(listImageIds[29],
                    "cd872f3c-tien-tieu-duong-co-the-dan-den-dai-thao-duong-loai-2-neu-khong-duoc-kiem-soat-tot._yfhhps",
                    "https://res.cloudinary.com/dc4eascme/image/upload/v1755438598/diabetesdoctor/cd872f3c-tien-tieu-duong-co-the-dan-den-dai-thao-duong-loai-2-neu-khong-duoc-kiem-soat-tot._yfhhps.jpg",
                    MediaType.Image, userId),
                Media.CreateForSeedData(listImageIds[30],
                    "23ecb305-ket-qua-do-cua-ban-la-11778-mmhg-co-chi-so-tam-thu-la-117-mmhg-va-tam-truong-la-78-mmhg_nv7vln",
                    "https://res.cloudinary.com/dc4eascme/image/upload/v1755439654/diabetesdoctor/23ecb305-ket-qua-do-cua-ban-la-11778-mmhg-co-chi-so-tam-thu-la-117-mmhg-va-tam-truong-la-78-mmhg_nv7vln.jpg",
                    MediaType.Image, userId),
            };
            await context.Medias.InsertManyAsync(medias);
        }
    }
}