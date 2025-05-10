namespace E_commerce.SQL.Queries
{
    public static class ConversationQueries
    {
        //Thêm 
        public static string Add =>
            @"INSERT INTO Conversation(conversation_name)
            VALUES(@conversation_name);";
        
        //Lấy tất cả
        public static string GetAll =>
            @"SELECT conversation_id, conversation_name
            FROM Conversation;";
        
        //Lấy dựa trên conversation_id
        public static string findByID => 
            @"SELECT conversation_id, conversation_name
            FROM Conversation
            WHERE conversation_id = @conversation_id;";

        //Cập nhật
        public static string UpdateByID =>
            @"UPDATE Conversation
            SET conversation_name = @conversation_name
            WHERE conversation_id = @conversation_id;";
        
        //Xóa 
        public static string DeleteByID =>
            @"DELETE FROM Conversation
            WHERE conversation_id = @conversation_id;";


        //Cập nhật - PATCH
        public static string PatchConversation => @"
            UPDATE Conversation
            SET conversation_name = COALESCE(@conversation_name, conversation_name)
            WHERE conversation_id = @conversation_id;
        ";
    }
}