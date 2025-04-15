namespace E_commerce.SQL.Queries
{
    public static class RankQueries
    {
        public static string AllRanks => "SELECT * FROM `Rank`";

        public static string FindByID => "SELECT * FROM `Rank` WHERE rank_id = @rank_id";

        public static string AddRank => 
            "INSERT INTO `Rank`(rank_id, rank_name, rating_point, `describe`)"+
            "VALUES(@rank_id, @rank_name, @rating_point, @describe);";

        public static string UpdateRank => 
            "UPDATE `Rank` SET rank_name=@rank_name, rating_point=@rating_point, `describe`=@describe "+
            "WHERE rank_id=@rank_id;";

        public static string DeleteRank => 
            "DELETE FROM `Rank` WHERE rank_id = @rank_id;";

        public static string PatchRank => 
            @"UPDATE `Rank` SET
            rank_name = COALESCE(@rank_name, rank_name), 
            rating_point = COALESCE(@rating_point, rating_point),
            `describe` = COALESCE(@describe, `describe`)
            WHERE rank_id = @rank_id;";
    }
}