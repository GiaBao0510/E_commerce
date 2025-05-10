namespace E_commerce.SQL.Queries
{
    public static class MessageQueries
    {
        #region  ===[CRUD]===
        // Create
        public static string cretae =>
            @"INSERT INTO Message(mess_id,`text`,send_date,from_number,conversation_id)
            VALUES(@mess_id,@text,@send_date,@from_number,@conversation_id)";
        
        // Read All
        public static string GetAll =>
            @"SELECT *
            FROM Message;";
        
        // Read By ID
        public static string FindByID =>
            @"SELECT *
            FROM Message
            WHERE mess_id=@mess_id;";

        // Update Text By mess_id
        public static string UpdateByID =>
            @"UPDATE Message SET
            	`text`=@text
            WHERE mess_id=@mess_id;";

        // Delete By mess_id
        public static string  DeleteByID => 
            @"DELETE FROM Message
            WHERE mess_id=@mess_id;";
        
        // Update (PATCH) By mess_id
        public static string UpdatePatchByID => 
         @"UPDATE Message SET
            `text`= COALESCE(@text,`text`),
        WHERE mess_id=@mess_id;";
        
        #endregion

        //Lấy danh sách các tin nhắn dựa trên cuộc hội thoại (one to one)
        public static string ListOfMessagesByConversationID =>
            @"SELECT mg.mess_id, mg.text, mg.send_date, mg.send_date,mg.from_number
            FROM Message mg
            WHERE mg.conversation_id = @conversation_id;";

        //Lấy danh sách các tin nhắn dựa trên Group Chat ID
        public static string listOfMessagesByGroupChatID =>
            @"SELECT mg.mess_id, mg.text, mg.send_date, mg.send_date,mg.from_number
            FROM GroupChat gc
            JOIN Message mg ON mg.conversation_id = gc.conversation_id
            WHERE gc.group_id = @group_id;";
    }
}