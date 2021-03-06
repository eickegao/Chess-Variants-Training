﻿using ChessVariantsTraining.Models.Variant960;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ChessVariantsTraining.MemoryRepositories.Variant960
{
    public class LobbySeekRepository : ILobbySeekRepository
    {
        List<LobbySeek> seeks = new List<LobbySeek>();
        bool shouldCleanup = true;
        ILobbySocketHandlerRepository socketHandlerRepository;

        public LobbySeekRepository(ILobbySocketHandlerRepository _socketHandlerRepository)
        {
            socketHandlerRepository = _socketHandlerRepository;
            Thread cleanupThread = new Thread(ScheduledCleanups);
            cleanupThread.Start();
        }

        public LobbySeek Get(string id)
        {
            return seeks.FirstOrDefault(x => x.ID == id);
        }

        public async Task<string> Add(LobbySeek seek)
        {
            string id;
            do
            {
                id = Guid.NewGuid().ToString().Split('-')[0];
            } while (seeks.Any(x => x.ID == id));
            seek.ID = id;
            seek.LatestBump = DateTime.UtcNow;
            seeks.Add(seek);
            await socketHandlerRepository.SendSeekAddition(seek);
            return id;
        }

        public async Task Remove(string id, GamePlayer client)
        {
            LobbySeek found = seeks.FirstOrDefault(x => x.ID == id);
            if (found == null) return;
            if (!found.Owner.Equals(client))
            {
                return;
            }

            seeks.RemoveAll(x => x.ID == id);
            await socketHandlerRepository.SendSeekRemoval(id);
        }

        public void Bump(string id, GamePlayer client)
        {
            LobbySeek toBump = seeks.Find(x => x.ID == id);
            if (toBump == null) return;
            if (!toBump.Owner.Equals(client))
            {
                return;
            }
            toBump.LatestBump = DateTime.UtcNow;
        }

        public List<LobbySeek> GetShallowCopy()
        {
            return new List<LobbySeek>(seeks);
        }

        void RemoveOldSeeks()
        {
            List<LobbySeek> old = seeks.Where(x => (DateTime.UtcNow - x.LatestBump).TotalSeconds > 10).ToList();
            foreach (LobbySeek seek in old)
            {
                socketHandlerRepository.SendSeekRemoval(seek.ID);
            }
            seeks.RemoveAll(x => (DateTime.UtcNow - x.LatestBump).TotalSeconds > 10);
        }

        void ScheduledCleanups()
        {
            while (shouldCleanup)
            {
                RemoveOldSeeks();
                Thread.Sleep(4000);
            }
        }
    }
}
