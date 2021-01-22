using System;
using System.Collections.Generic;
using Microsoft.JSInterop;
using System.Threading.Tasks;
using DnetIndexedDb.Models;
using System.Text.Json;

namespace DnetIndexedDb
{
    public class IndexedDbInterop
    {
        private readonly IJSRuntime _jsRuntime;

        private readonly IndexedDbOptions _indexedDbDatabaseOptions;

        private IndexedDbDatabaseModel _indexedDbDatabaseModel;

        public IndexedDbInterop(IJSRuntime jsRuntime, IndexedDbOptions indexedDbDatabaseOptions)
        {
            _jsRuntime = jsRuntime;
            _indexedDbDatabaseOptions = indexedDbDatabaseOptions;

            SetDbModel();
        }

        private void SetDbModel()
        {

            var option = _indexedDbDatabaseOptions.GetExtension<CoreOptionsExtension>();

            if (option != null)
            {
                _indexedDbDatabaseModel = option.IndexedDbDatabaseModel;

                _indexedDbDatabaseModel.DbModelGuid = Guid.NewGuid().ToString();
            }
            else
            {
                throw new NullReferenceException("IndexedDB Database Model not configured. Add one in AddIndexedDbDatabase method");
            }
        }

        /// <summary>
        /// Create, Open or Upgrade IndexedDb Database
        /// </summary>
        /// <returns></returns>
        public async ValueTask<int> OpenIndexedDb()
        {
            var dbModelId = await _jsRuntime.InvokeAsync<int>("dnetindexeddbinterop.openDb", _indexedDbDatabaseModel);

            if (dbModelId != -1) _indexedDbDatabaseModel.DbModelId = dbModelId;

            return dbModelId;
        }

        /// <summary>
        /// Delete IndexedDb Database
        /// </summary>
        /// <returns></returns>
        public async ValueTask<string> DeleteIndexedDb()
        {
            return await _jsRuntime.InvokeAsync<string>("dnetindexeddbinterop.deleteDb", _indexedDbDatabaseModel);
        }

        /// <summary>
        /// Add records to a given data store
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="objectStoreName"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        public async ValueTask<string> AddItems<TEntity>(string objectStoreName, List<TEntity> items)
        {
            return await _jsRuntime.InvokeAsync<string>("dnetindexeddbinterop.addItems", _indexedDbDatabaseModel, objectStoreName, items);
        }

        /// <summary>
        /// Update records in a given data store
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="objectStoreName"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        public async ValueTask<string> UpdateItems<TEntity>(string objectStoreName, List<TEntity> items)
        {
            return await _jsRuntime.InvokeAsync<string>("dnetindexeddbinterop.updateItems", _indexedDbDatabaseModel, objectStoreName, items);
        }

        /// <summary>
        /// Update records in a given data store by keys
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="objectStoreName"></param>
        /// <param name="items"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        public async ValueTask<string> UpdateItemsByKey<TEntity>(string objectStoreName, List<TEntity> items, List<int> keys)
        {
            return await _jsRuntime.InvokeAsync<string>("dnetindexeddbinterop.updateItemsByKey", _indexedDbDatabaseModel, objectStoreName, items, keys);
        }

        /// <summary>
        /// Return a record in a given data store by its key
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="key"></param>
        /// <param name="objectStoreName"></param>
        /// <returns></returns>
        public async ValueTask<TEntity> GetByKey<TKey, TEntity>(string objectStoreName, TKey key)
        {
            return await _jsRuntime.InvokeAsync<TEntity>("dnetindexeddbinterop.getByKey", _indexedDbDatabaseModel, objectStoreName, key);
        }

        /// <summary>
        /// Delete a record in a given data store by its key
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="key"></param>
        /// <param name="objectStoreName"></param>
        /// <returns></returns>
        public async ValueTask<string> DeleteByKey<TKey>(string objectStoreName, TKey key)
        {
            return await _jsRuntime.InvokeAsync<string>("dnetindexeddbinterop.deleteByKey", _indexedDbDatabaseModel, objectStoreName, key);
        }

        /// <summary>
        /// Delete all records in a given data store
        /// </summary>
        /// <param name="objectStoreName"></param>
        /// <returns></returns>
        public async ValueTask<string> DeleteAll(string objectStoreName)
        {
            return await _jsRuntime.InvokeAsync<string>("dnetindexeddbinterop.deleteAll", _indexedDbDatabaseModel, objectStoreName);
        }

        /// <summary>
        /// Return all records in a given data store
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="objectStoreName"></param>
        /// <returns></returns>
        public async ValueTask<List<TEntity>> GetAll<TEntity>(string objectStoreName)
        {
            return await _jsRuntime.InvokeAsync<List<TEntity>>("dnetindexeddbinterop.getAll", _indexedDbDatabaseModel, objectStoreName);
        }

        /// <summary>
        /// Return some records in a given data store by key
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="objectStoreName"></param>
        /// <param name="lowerBound"></param>
        /// <param name="upperBound"></param>
        /// <returns></returns>
        public async ValueTask<List<TEntity>> GetRange<TKey, TEntity>(string objectStoreName, TKey lowerBound, TKey upperBound)
        {
            return await _jsRuntime.InvokeAsync<List<TEntity>>("dnetindexeddbinterop.getRange", _indexedDbDatabaseModel, objectStoreName, lowerBound, upperBound);
        }

        /// <summary>
        /// Return some records in a given data store by index
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="objectStoreName"></param>
        /// <param name="lowerBound"></param>
        /// <param name="upperBound"></param>
        /// <param name="dbIndex"></param>
        /// <param name="isRange"></param>
        /// <returns></returns>
        public async ValueTask<List<TEntity>> GetByIndex<TKey, TEntity>(string objectStoreName, TKey lowerBound, TKey upperBound, string dbIndex, bool isRange)
        {
            return await _jsRuntime.InvokeAsync<List<TEntity>>("dnetindexeddbinterop.getByIndex", _indexedDbDatabaseModel, objectStoreName, lowerBound, upperBound, dbIndex, isRange);
        }

        /// <summary>
        /// Returns the max value in the given data store's index
        /// </summary>
        /// <typeparam name="TIndex"></typeparam>
        /// <param name="objectStoreName"></param>
        /// <param name="dbIndex"></param>
        /// <returns></returns>
        public async ValueTask<TIndex> GetMaxIndex<TIndex>(string objectStoreName, string dbIndex)
        {
            return await GetExtent<TIndex>(objectStoreName, dbIndex, "Max");
        }

        /// <summary>
        /// Returns the max value in the given data store's key
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="objectStoreName"></param>
        /// <returns></returns>
        public async ValueTask<TKey> GetMaxKey<TKey>(string objectStoreName)
        {
            return await GetExtent<TKey>(objectStoreName, null, "Max");
        }

        /// <summary>
        /// Returns the minimum value in the given data store's index 
        /// </summary>
        /// <typeparam name="TIndex"></typeparam>
        /// <param name="objectStoreName"></param>
        /// <param name="dbIndex"></param>
        /// <returns></returns>
        public async ValueTask<TIndex> GetMinIndex<TIndex>(string objectStoreName, string dbIndex)
        {
            return await GetExtent<TIndex>(objectStoreName, dbIndex, "Min");
        }

        /// <summary>
        /// Returns the minimum value in the given data store's key 
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="objectStoreName"></param>
        /// <returns></returns>
        public async ValueTask<TKey> GetMinKey<TKey>(string objectStoreName)
        {
            return await GetExtent<TKey>(objectStoreName, null, "Min");
        }

        private async ValueTask<T> GetExtent<T>(string objectStoreName, string dbIndex, string extentType)
        {
            var result = await _jsRuntime.InvokeAsync<JsonElement>("dnetindexeddbinterop.getExtent", _indexedDbDatabaseModel, objectStoreName, dbIndex, extentType);

            var resultJson = result.GetRawText();
            if (resultJson == "null")
            {
                return default;
            }
            else
            {
                return JsonSerializer.Deserialize<T>(resultJson);
            }
        }
    }
}
