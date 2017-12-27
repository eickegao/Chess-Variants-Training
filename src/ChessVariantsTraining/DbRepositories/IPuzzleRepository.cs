﻿using ChessVariantsTraining.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChessVariantsTraining.DbRepositories
{
    public interface IPuzzleRepository
    {
        bool Add(Puzzle puzzle);
        Puzzle Get(int id);
        Puzzle GetOneRandomly(List<int> excludedIds, string variant, int? userId, double nearRating);
        DeleteResult Remove(int id);
        DeleteResult RemoveAllBy(int author);
        bool UpdateRating(int id, Rating newRating);
        List<Puzzle> InReview();
        bool Approve(int id, int reviewer);
        bool Reject(int id, int reviewer);
        Puzzle FindByFenAndVariant(string fen, string variant);

        Task<bool> AddAsync(Puzzle puzzle);
        Task<Puzzle> GetAsync(int id);
        Task<Puzzle> GetOneRandomlyAsync(List<int> excludedIds, string variant, int? userId, double nearRating);
        Task<DeleteResult> RemoveAsync(int id);
        Task<DeleteResult> RemoveAllByAsync(int author);
        Task<bool> UpdateRatingAsync(int id, Rating newRating);
        Task<List<Puzzle>> InReviewAsync();
        Task<bool> ApproveAsync(int id, int reviewer);
        Task<bool> RejectAsync(int id, int reviewer);
        Task<Puzzle> FindByFenAndVariantAsync(string fen, string variant);
    }
}
