namespace E_commerce.SQL.Queries
{
    public static class GroupChatQueries
    {
        //Get All
        public static string GetAll =>
            @"SELECT * FROM GroupChat;";

        //Get by GroupID
        public static string FindByID =>
            @"SELECT * FROM GroupChat WHERE group_id = @group_id;";
        
        //Delete by GroupID
        public static string DeleteByID =>
            @"DELETE FROM GroupChat WHERE group_id = @group_id;";

        //Put by GroupID
        public static string UpdateByID =>
            @"UPDATE GroupChat SET 
                group_name = @group_name,
                is_group = @is_group,
                join_date = @join_date,
                user_id = @user_id,
                conversation_id = @conversation_id
            WHERE group_id = @group_id;";
            
        //Add
        public static string Add =>
            @"INSERT INTO GroupChat(group_name, is_group, join_date, user_id, conversation_id)
            VALUES(@group_name, @is_group, @join_date, @user_id, @conversation_id)";

        //Patch by GroupID
        public static string PatchByID =>
            @"UPDATE GroupChat SET 
                group_name = COALESCE(@group_name, group_name),
                is_group = COALESCE(@is_group,is_group),
                join_date = COALESCE(@join_date, join_date),
                user_id = COALESCE(@user_id,user_id),
                conversation_id = COALESCE(@conversation_id,conversation_id)
            WHERE group_id = @group_id;";

        //Danh sách các người dùng trong nhóm dựa trên ConversationID
        public static string GetUserByConversationID =>
            @"SELECT u.user_id, u.user_name, u.date_of_birth, u.address, u.phone_num,
            	u.email, u.is_block, u.is_delete
            FROM `User` u
            JOIN GroupChat g ON u.user_id = g.user_id
            WHERE g.conversation_id = @conversation_id;";

        //Danh sách các cuộc hội thoại của người dùng dựa trên UserID
        public static string GetConversationByUserID =>
            @"SELECT c.conversation_id, c.conversation_name
            FROM Conversation c
            JOIN GroupChat g ON c.conversation_id = g.conversation_id
            WHERE g.user_id = @user_id;";
    }
}