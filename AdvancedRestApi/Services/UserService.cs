using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdvancedRestApi.Interfaces;
using AdvancedRestApi.Data;
using AdvancedRestApi.Models;
using Microsoft.EntityFrameworkCore;
using AdvancedRestApi.DTOs;
using AutoMapper;
using MongoDB.Driver;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Configuration;

namespace AdvancedRestApi.Services
{
    public class UserService : IUser
    {
        //private readonly UserDbContext _dbContext;

        private IMongoCollection<User> usersCollection;

        private readonly IMapper _mapper;

        public UserService(/*UserDbContext dbContext,*/ IMapper mapper, IConfiguration config)
        {
            //_dbContext = dbContext;
            _mapper = mapper;

            var mongoClient = new MongoClient(config.GetConnectionString("MongoDBConnection"));

            var mongoDatabase = mongoClient.GetDatabase("UsersDb");

            usersCollection = mongoDatabase.GetCollection<User>("Users");
        }

        public async Task<(bool IsSuccess, List<UserDTO> User, string ErrorMessage)> GetAllUsers()
        {
            //var users = await _dbContext.Users.ToListAsync();

            var users = await usersCollection.Find(u => true).ToListAsync();

            if (users != null)
            {
                var result = _mapper.Map<List<UserDTO>>(users);
                return (true, result, null);
            }

            return (false, null, "No users found");
        }

        public async Task<(bool IsSuccess, UserDTO User, string ErrorMessage)> GetUserById(Guid id)
        {
            //var user = await _dbContext.Users.FindAsync(id);

            var user = await usersCollection.Find(u => u.Id == id).FirstOrDefaultAsync();

            if (user != null)
            {
                var result = _mapper.Map<UserDTO>(user);
                return (true, result, null);
            }

            return (false, null, "No user found");
        }

        public async Task<(bool IsSuccess, string ErrorMessage)> AddUser(UserDTO userdto)
        {
            if (userdto != null)
            {
                var user = _mapper.Map<User>(userdto);

                //await _dbContext.Users.AddAsync(user);
                //await _dbContext.SaveChangesAsync();

                await usersCollection.InsertOneAsync(user);

                return (true, null);
            }

            return (false, "Please provide the user data");
        }

        public async Task<(bool IsSuccess, string ErrorMessage)> UpdateUser(Guid id, UserDTO userdto)
        {
            //var userObj = await _dbContext.Users.FindAsync(id);

            var userObj = await this.usersCollection.Find(u => u.Id == id).FirstOrDefaultAsync();

            if (userObj != null)
            {
                var user = _mapper.Map<User>(userdto);
                //userObj.Name = user.Name;
                //userObj.Address = user.Address;
                //userObj.Phone = user.Phone;
                //userObj.BloodGroup = user.BloodGroup;
                //await _dbContext.SaveChangesAsync();

                await this.usersCollection.ReplaceOneAsync(u => u.Id == id, user);

                return (true, null);
            }

            return (false, "User not found");
        }

        public async Task<(bool IsSuccess, string ErrorMessage)> DeleteUser(Guid id)
        {
            //var user = await _dbContext.Users.FindAsync(id);

            var user = await usersCollection.Find(u => u.Id == id).FirstOrDefaultAsync();

            if (user != null)
            {
                //_dbContext.Users.Remove(user);
                //await _dbContext.SaveChangesAsync();
                await usersCollection.DeleteOneAsync(u => u.Id == id);
                return (true, null);
            }

            return (false, "No user found");
        }
    }
}