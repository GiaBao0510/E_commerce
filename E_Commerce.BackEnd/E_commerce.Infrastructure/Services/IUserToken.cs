namespace E_commerce.Infrastructure.Services
{
    /*
        Ở đây sẽ mô tả mối quan hệ giữa người dùng và refreshtoken được lưu trên Redis.
        - (1 người dùng sẽ có nhiều refreshtoken) -> Vì người dùng có 1 thiết bị: Chính vì vậy sẽ dùng 1 list. Với key là Uid và các values là refreshtoken
        - (refreshtoken sẽ lưu trên một người dùng ) -> Vì 1 refreshtoken sẽ thuộc về 1 người dùng. Chính vì vậy sẽ dùng 1 dictionary với key là refreshtoken và value là Uid

        - Sẽ có các phương thức như sau:
            + Tìm refreshtoken theo Uid (key: Uid, value: refreshtoken)
            + Tìm Uid theo  refreshtoken (Key: refreshtoken, value: Uid)
            + Xóa refreshtoken cần xóa dựa trên Uid (key: Uid, value: refreshtoken) - Xóa Uid dựa trên refreshtoken (Key: refreshtoken, value: Uid)
            + 
    */
    public interface IUserToken
    {
        
    }
}