﻿//using CSRedis;
//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using System.Threading;

//public abstract partial class RedisHelper {
//	private static CSRedisClient _instance;
//	/// <summary>
//	/// CSRedisClient 静态实例，使用前请初始化
//	/// RedisHelper.Initialization(
//	///		csredis: new CSRedis.CSRedisClient(\"127.0.0.1:6379,pass=123,defaultDatabase=13,poolsize=50,ssl=false,writeBuffer=10240,prefix=key前辍\"), 
//	///		serialize: value => Newtonsoft.Json.JsonConvert.SerializeObject(value), 
//	///		deserialize: (data, type) => Newtonsoft.Json.JsonConvert.DeserializeObject(data, type))
//	/// </summary>
//	public static CSRedisClient Instance {
//		get {
//			if (_instance == null) throw new Exception("使用前请初始化 RedisHelper.Initialization(new CSRedis.CSRedisClient(\"127.0.0.1:6379,pass=123,defaultDatabase=13,poolsize=50,ssl=false,writeBuffer=10240,prefix=key前辍\");");
//			return _instance;
//		}
//	}
//	public static Dictionary<string, RedisClientPool> Nodes => Instance.Nodes;
//	private static DateTime dt1970 = new DateTime(1970, 1, 1);

//	/// <summary>
//	/// 初始化csredis静态访问类
//	/// RedisHelper.Initialization(new CSRedis.CSRedisClient(
//	///		connectionString: \"127.0.0.1:6379,pass=123,defaultDatabase=13,poolsize=50,ssl=false,writeBuffer=10240,prefix=key前辍\"), 
//	///		serialize: value => Newtonsoft.Json.JsonConvert.SerializeObject(value), 
//	///		deserialize: (data, type) => Newtonsoft.Json.JsonConvert.DeserializeObject(data, type)))
//	/// </summary>
//	/// <param name="csredis"></param>
//	public static void Initialization(CSRedisClient csredis) {
//		_instance = csredis;
//	}

//	/// <summary>
//	/// 缓存壳
//	/// </summary>
//	/// <typeparam name="T">缓存类型</typeparam>
//	/// <param name="key">不含prefix前辍</param>
//	/// <param name="timeoutSeconds">缓存秒数</param>
//	/// <param name="getData">获取源数据的函数</param>
//	/// <returns></returns>
//	public static T CacheShell<T>(string key, int timeoutSeconds, Func<T> getData) => Instance.CacheShell(key, timeoutSeconds, getData);
//	/// <summary>
//	/// 缓存壳(哈希表)
//	/// </summary>
//	/// <typeparam name="T">缓存类型</typeparam>
//	/// <param name="key">不含prefix前辍</param>
//	/// <param name="field">字段</param>
//	/// <param name="timeoutSeconds">缓存秒数</param>
//	/// <param name="getData">获取源数据的函数</param>
//	/// <returns></returns>
//	public static T CacheShell<T>(string key, string field, int timeoutSeconds, Func<T> getData) => Instance.CacheShell(key, field, timeoutSeconds, getData);

//	/// <summary>
//	/// 缓存壳(哈希表)，将 fields 每个元素存储到单独的缓存片，实现最大化复用
//	/// </summary>
//	/// <typeparam name="T">缓存类型</typeparam>
//	/// <param name="key">不含prefix前辍</param>
//	/// <param name="fields">字段</param>
//	/// <param name="timeoutSeconds">缓存秒数</param>
//	/// <param name="getData">获取源数据的函数，输入参数是没有缓存的 fields，返回值应该是 (field, value)[]</param>
//	/// <returns></returns>
//	public static T[] CacheShell<T>(string key, string[] fields, int timeoutSeconds, Func<string[], (string, T)[]> getData) => Instance.CacheShell(key, fields, timeoutSeconds, getData);

//	/// <summary>
//	/// 创建管道传输
//	/// </summary>
//	/// <param name="handler"></param>
//	/// <returns></returns>
//	public static object[] StartPipe(Action<CSRedisClientPipe> handler) => Instance.StartPipe(handler);

//	/// <summary>
//	/// 创建管道传输，打包提交如：RedisHelper.StartPipe().Set("a", "1").HashSet("b", "f", "2").EndPipe();
//	/// </summary>
//	/// <returns></returns>
//	[Obsolete("警告：本方法必须有 EndPipe() 提交，否则会造成连接池耗尽。")]
//	public static CSRedisClientPipe StartPipe() => Instance.StartPipe();

//	/// <summary>
//	/// 设置指定 key 的值
//	/// </summary>
//	/// <param name="key">不含prefix前辍</param>
//	/// <param name="value">字符串值</param>
//	/// <param name="expireSeconds">过期(秒单位)</param>
//	/// <param name="exists">Nx, Xx</param>
//	/// <returns></returns>
//	public static bool Set(string key, string value, int expireSeconds = -1, CSRedisExistence? exists = null) => Instance.Set(key, value, expireSeconds, exists);
//	/// <summary>
//	/// 设置指定 key 的值(字节流)
//	/// </summary>
//	/// <param name="key">不含prefix前辍</param>
//	/// <param name="value">字节流</param>
//	/// <param name="expireSeconds">过期(秒单位)</param>
//	/// <param name="exists">Nx, Xx</param>
//	/// <returns></returns>
//	public static bool SetBytes(string key, byte[] value, int expireSeconds = -1, CSRedisExistence? exists = null) => Instance.SetBytes(key, value, expireSeconds, exists);
//	/// <summary>
//	/// 只有在 key 不存在时设置 key 的值。
//	/// </summary>
//	/// <param name="key">不含prefix前辍</param>
//	/// <param name="value">字符串值</param>
//	/// <returns></returns>
//	public static bool SetNx(string key, string value) => Instance.SetNx(key, value);
//	/// <summary>
//	/// 同时设置一个或多个 key-value 对。
//	/// </summary>
//	/// <param name="keyValues">key1 value1 [key2 value2]</param>
//	/// <returns></returns>
//	public static bool MSet(params string[] keyValues) => Instance.MSet(keyValues);
//	/// <summary>
//	/// 同时设置一个或多个 key-value 对，当且仅当所有给定 key 都不存在。警告：群集模式下，若keys分散在多个节点时，将报错
//	/// </summary>
//	/// <param name="keyValues">key1 value1 [key2 value2]</param>
//	/// <returns></returns>
//	public static bool MSetNx(params string[] keyValues) => Instance.MSetNx(keyValues);
//	/// <summary>
//	/// 获取指定 key 的值
//	/// </summary>
//	/// <param name="key">不含prefix前辍</param>
//	/// <returns></returns>
//	public static string Get(string key) => Instance.Get(key);
//	/// <summary>
//	/// 获取多个指定 key 的值(数组)
//	/// </summary>
//	/// <param name="key">不含prefix前辍</param>
//	/// <returns></returns>
//	public static string[] MGet(params string[] key) => Instance.MGet(key);
//	/// <summary>
//	/// 获取多个指定 key 的值(数组)
//	/// </summary>
//	/// <param name="key">不含prefix前辍</param>
//	/// <returns></returns>
//	public static string[] GetStrings(params string[] key) => Instance.GetStrings(key);
//	/// <summary>
//	/// 获取指定 key 的值(字节流)
//	/// </summary>
//	/// <param name="key">不含prefix前辍</param>
//	/// <returns></returns>
//	public static byte[] GetBytes(string key) => Instance.GetBytes(key);
//	/// <summary>
//	/// 用于在 key 存在时删除 key
//	/// </summary>
//	/// <param name="key">不含prefix前辍</param>
//	/// <returns></returns>
//	public static long Remove(params string[] key) => Instance.Remove(key);
//	/// <summary>
//	/// 检查给定 key 是否存在
//	/// </summary>
//	/// <param name="key">不含prefix前辍</param>
//	/// <returns></returns>
//	public static bool Exists(string key) => Instance.Exists(key);
//	/// <summary>
//	/// 将 key 所储存的值加上给定的增量值（increment）
//	/// </summary>
//	/// <param name="key">不含prefix前辍</param>
//	/// <param name="value">增量值(默认=1)</param>
//	/// <returns></returns>
//	public static long Increment(string key, long value = 1) => Instance.Increment(key, value);
//	/// <summary>
//	/// 为给定 key 设置过期时间
//	/// </summary>
//	/// <param name="key">不含prefix前辍</param>
//	/// <param name="expire">过期时间</param>
//	/// <returns></returns>
//	public static bool Expire(string key, TimeSpan expire) => Instance.Expire(key, expire);
//	/// <summary>
//	/// 以秒为单位，返回给定 key 的剩余生存时间
//	/// </summary>
//	/// <param name="key">不含prefix前辍</param>
//	/// <returns></returns>
//	public static long Ttl(string key) => Instance.Ttl(key);
//	/// <summary>
//	/// 执行脚本
//	/// </summary>
//	/// <param name="script">脚本</param>
//	/// <param name="key">不含prefix前辍</param>
//	/// <param name="args">参数</param>
//	/// <returns></returns>
//	public static object Eval(string script, string key, params object[] args) => Instance.Eval(script, key, args);
//	/// <summary>
//	/// 查找所有符合给定模式( pattern)的 key
//	/// </summary>
//	/// <param name="pattern">如：runoob*</param>
//	/// <returns></returns>
//	public static string[] Keys(string pattern) => Instance.Keys(pattern);
//	/// <summary>
//	/// Redis Publish 命令用于将信息发送到指定的频道
//	/// </summary>
//	/// <param name="channel">频道名</param>
//	/// <param name="data">消息文本</param>
//	/// <returns></returns>
//	public static long Publish(string channel, string data) => Instance.Publish(channel, data);
//	/// <summary>
//	/// 订阅，根据分区规则返回SubscribeObject，Subscribe(("chan1", msg => Console.WriteLine(msg.Body)), ("chan2", msg => Console.WriteLine(msg.Body)))
//	/// </summary>
//	/// <param name="channels">频道和接收器</param>
//	/// <returns>返回可停止订阅的对象</returns>
//	public static CSRedisClient.SubscribeObject Subscribe(params (string, Action<CSRedisClient.SubscribeMessageEventArgs>)[] channels) => Instance.Subscribe(channels);
//	/// <summary>
//	/// 模糊订阅，订阅所有分区节点(同条消息只处理一次），返回SubscribeObject，PSubscribe(new [] { "chan1*", "chan2*" }, msg => Console.WriteLine(msg.Body))
//	/// </summary>
//	/// <param name="channelPatterns">模糊频道</param>
//	/// <param name="pmessage">接收器</param>
//	/// <returns>返回可停止模糊订阅的对象</returns>
//	public static CSRedisClient.PSubscribeObject PSubscribe(string[] channelPatterns, Action<CSRedisClient.PSubscribePMessageEventArgs> pmessage) => Instance.PSubscribe(channelPatterns, pmessage);
//	#region Hash 操作
//	/// <summary>
//	/// 同时将多个 field-value (域-值)对设置到哈希表 key 中，value 可以是 string 或 byte[]
//	/// </summary>
//	/// <param name="key">不含prefix前辍</param>
//	/// <param name="keyValues">field1 value1 [field2 value2]</param>
//	/// <returns></returns>
//	public static string HashSet(string key, params object[] keyValues) => Instance.HashSet(key, keyValues);
//	/// <summary>
//	/// 同时将多个 field-value (域-值)对设置到哈希表 key 中，value 可以是 string 或 byte[]
//	/// </summary>
//	/// <param name="key">不含prefix前辍</param>
//	/// <param name="expire">过期时间</param>
//	/// <param name="keyValues">field1 value1 [field2 value2]</param>
//	/// <returns></returns>
//	public static string HashSetExpire(string key, TimeSpan expire, params object[] keyValues) => Instance.HashSetExpire(key, expire, keyValues);
//	/// <summary>
//	/// 只有在字段 field 不存在时，设置哈希表字段的值。
//	/// </summary>
//	/// <param name="key">不含prefix前辍</param>
//	/// <param name="field">字段</param>
//	/// <param name="value">值(string 或 byte[])</param>
//	/// <returns></returns>
//	public static bool HashSetNx(string key, string field, object value) => Instance.HSetNx(key, field, value);
//	/// <summary>
//	/// 获取存储在哈希表中指定字段的值
//	/// </summary>
//	/// <param name="key">不含prefix前辍</param>
//	/// <param name="field">字段</param>
//	/// <returns></returns>
//	public static string HashGet(string key, string field) => Instance.HGet(key, field);
//	/// <summary>
//	/// 获取存储在哈希表中指定字段的值，返回 byte[]
//	/// </summary>
//	/// <param name="key">不含prefix前辍</param>
//	/// <param name="field">字段</param>
//	/// <returns>byte[]</returns>
//	public static byte[] HashGetBytes(string key, string field) => Instance.HashGetBytes(key, field);
//	/// <summary>
//	/// 获取存储在哈希表中多个字段的值
//	/// </summary>
//	/// <param name="key">不含prefix前辍</param>
//	/// <param name="fields">字段</param>
//	/// <returns></returns>
//	public static string[] HashMGet(string key, params string[] fields) => Instance.HMGet(key, fields);
//	/// <summary>
//	/// 获取存储在哈希表中多个字段的值，每个 field 的值类型返回 byte[]
//	/// </summary>
//	/// <param name="key">不含prefix前辍</param>
//	/// <param name="fields">字段</param>
//	/// <returns>byte[][]</returns>
//	public static byte[][] HashMGetBytes(string key, params string[] fields) => Instance.HashMGetBytes(key, fields);
//	/// <summary>
//	/// 为哈希表 key 中的指定字段的整数值加上增量 increment
//	/// </summary>
//	/// <param name="key">不含prefix前辍</param>
//	/// <param name="field">字段</param>
//	/// <param name="value">增量值(默认=1)</param>
//	/// <returns></returns>
//	public static long HashIncrement(string key, string field, long value = 1) => Instance.HIncrBy(key, field, value);
//	/// <summary>
//	/// 为哈希表 key 中的指定字段的整数值加上增量 increment
//	/// </summary>
//	/// <param name="key">不含prefix前辍</param>
//	/// <param name="field">字段</param>
//	/// <param name="value">增量值(默认=1)</param>
//	/// <returns></returns>
//	public static double HashIncrementFloat(string key, string field, double value = 1) => Instance.HIncrByFloat(key, field, value);
//	/// <summary>
//	/// 删除一个或多个哈希表字段
//	/// </summary>
//	/// <param name="key">不含prefix前辍</param>
//	/// <param name="fields">字段</param>
//	/// <returns></returns>
//	public static long HashDelete(string key, params string[] fields) => Instance.HDel(key, fields);
//	/// <summary>
//	/// 查看哈希表 key 中，指定的字段是否存在
//	/// </summary>
//	/// <param name="key">不含prefix前辍</param>
//	/// <param name="field">字段</param>
//	/// <returns></returns>
//	public static bool HashExists(string key, string field) => Instance.HExists(key, field);
//	/// <summary>
//	/// 获取哈希表中字段的数量
//	/// </summary>
//	/// <param name="key">不含prefix前辍</param>
//	/// <returns></returns>
//	public static long HashLength(string key) => Instance.HLen(key);
//	/// <summary>
//	/// 获取在哈希表中指定 key 的所有字段和值
//	/// </summary>
//	/// <param name="key">不含prefix前辍</param>
//	/// <returns></returns>
//	public static Dictionary<string, string> HashGetAll(string key) => Instance.HGetAll(key);
//	/// <summary>
//	/// 获取所有哈希表中的字段
//	/// </summary>
//	/// <param name="key">不含prefix前辍</param>
//	/// <returns></returns>
//	public static string[] HashKeys(string key) => Instance.HKeys(key);
//	/// <summary>
//	/// 获取哈希表中所有值
//	/// </summary>
//	/// <param name="key">不含prefix前辍</param>
//	/// <returns></returns>
//	public static string[] HashVals(string key) => Instance.HVals(key);
//	#endregion

//	#region List 操作
//	/// <summary>
//	/// 它是 LPOP 命令的阻塞版本，当给定列表内没有任何元素可供弹出的时候，连接将被 BLPOP 命令阻塞，直到等待超时或发现可弹出元素为止，超时返回null。警告：群集模式下，若keys分散在多个节点时，将报错
//	/// </summary>
//	/// <param name="timeout">超时(秒)</param>
//	/// <param name="keys">一个或多个列表，不含prefix前辍</param>
//	/// <returns></returns>
//	public static string BLPop(int timeout, params string[] keys) => Instance.BLPop(timeout, keys);
//	/// <summary>
//	/// 它是 LPOP 命令的阻塞版本，当给定列表内没有任何元素可供弹出的时候，连接将被 BLPOP 命令阻塞，直到等待超时或发现可弹出元素为止，超时返回null。警告：群集模式下，若keys分散在多个节点时，将报错
//	/// </summary>
//	/// <param name="timeout"></param>
//	/// <param name="keys"></param>
//	/// <returns></returns>
//	public static (string key, string value)? BLPopWithKey(int timeout, params string[] keys) => Instance.BLPopWithKey(timeout, keys);
//	/// <summary>
//	/// 它是 RPOP 命令的阻塞版本，当给定列表内没有任何元素可供弹出的时候，连接将被 BRPOP 命令阻塞，直到等待超时或发现可弹出元素为止，超时返回null。警告：群集模式下，若keys分散在多个节点时，将报错
//	/// </summary>
//	/// <param name="timeout">超时(秒)</param>
//	/// <param name="keys">一个或多个列表，不含prefix前辍</param>
//	/// <returns></returns>
//	public static string BRPop(int timeout, params string[] keys) => Instance.BRPop(timeout, keys);
//	/// <summary>
//	/// 它是 RPOP 命令的阻塞版本，当给定列表内没有任何元素可供弹出的时候，连接将被 BRPOP 命令阻塞，直到等待超时或发现可弹出元素为止，超时返回null。警告：群集模式下，若keys分散在多个节点时，将报错
//	/// </summary>
//	/// <param name="timeout">超时(秒)</param>
//	/// <param name="keys">一个或多个列表，不含prefix前辍</param>
//	/// <returns></returns>
//	public static (string key, string value)? BRPopWithKey(int timeout, params string[] keys) => Instance.BRPopWithKey(timeout, keys);
//	/// <summary>
//	/// 通过索引获取列表中的元素
//	/// </summary>
//	/// <param name="key">不含prefix前辍</param>
//	/// <param name="index">索引</param>
//	/// <returns></returns>
//	public static string LIndex(string key, long index) => Instance.LIndex(key, index);
//	/// <summary>
//	/// 在列表的元素前面插入元素
//	/// </summary>
//	/// <param name="key">不含prefix前辍</param>
//	/// <param name="pivot">列表的元素</param>
//	/// <param name="value">新元素</param>
//	/// <returns></returns>
//	public static long LInsertBefore(string key, string pivot, string value) => Instance.LInsertBefore(key, pivot, value);
//	/// <summary>
//	/// 在列表的元素后面插入元素
//	/// </summary>
//	/// <param name="key">不含prefix前辍</param>
//	/// <param name="pivot">列表的元素</param>
//	/// <param name="value">新元素</param>
//	/// <returns></returns>
//	public static long LInsertAfter(string key, string pivot, string value) => Instance.LInsertAfter(key, pivot, value);
//	/// <summary>
//	/// 获取列表长度
//	/// </summary>
//	/// <param name="key">不含prefix前辍</param>
//	/// <returns></returns>
//	public static long LLen(string key) => Instance.LLen(key);
//	/// <summary>
//	/// 移出并获取列表的第一个元素
//	/// </summary>
//	/// <param name="key">不含prefix前辍</param>
//	/// <returns></returns>
//	public static string LPop(string key) => Instance.LPop(key);
//	/// <summary>
//	/// 移除并获取列表最后一个元素
//	/// </summary>
//	/// <param name="key">不含prefix前辍</param>
//	/// <returns></returns>
//	public static string RPop(string key) => Instance.RPop(key);
//	/// <summary>
//	/// 将一个或多个值插入到列表头部
//	/// </summary>
//	/// <param name="key">不含prefix前辍</param>
//	/// <param name="value">一个或多个值</param>
//	/// <returns></returns>
//	public static long LPush(string key, params string[] value) => Instance.LPush(key, value);
//	/// <summary>
//	/// 在列表中添加一个或多个值
//	/// </summary>
//	/// <param name="key">不含prefix前辍</param>
//	/// <param name="value">一个或多个值</param>
//	/// <returns></returns>
//	public static long RPush(string key, params string[] value) => Instance.RPush(key, value);
//	/// <summary>
//	/// 获取列表指定范围内的元素
//	/// </summary>
//	/// <param name="key">不含prefix前辍</param>
//	/// <param name="start">开始位置，0表示第一个元素，-1表示最后一个元素</param>
//	/// <param name="stop">结束位置，0表示第一个元素，-1表示最后一个元素</param>
//	/// <returns></returns>
//	public static string[] LRang(string key, long start, long stop) => Instance.LRang(key, start, stop);
//	/// <summary>
//	/// 根据参数 count 的值，移除列表中与参数 value 相等的元素
//	/// </summary>
//	/// <param name="key">不含prefix前辍</param>
//	/// <param name="count">移除的数量，大于0时从表头删除数量count，小于0时从表尾删除数量-count，等于0移除所有</param>
//	/// <param name="value">元素</param>
//	/// <returns></returns>
//	public static long LRem(string key, long count, string value) => Instance.LRem(key, count, value);
//	/// <summary>
//	/// 通过索引设置列表元素的值
//	/// </summary>
//	/// <param name="key">不含prefix前辍</param>
//	/// <param name="index">索引</param>
//	/// <param name="value">值</param>
//	/// <returns></returns>
//	public static bool LSet(string key, long index, string value) => Instance.LSet(key, index, value);
//	/// <summary>
//	/// 对一个列表进行修剪，让列表只保留指定区间内的元素，不在指定区间之内的元素都将被删除
//	/// </summary>
//	/// <param name="key">不含prefix前辍</param>
//	/// <param name="start">开始位置，0表示第一个元素，-1表示最后一个元素</param>
//	/// <param name="stop">结束位置，0表示第一个元素，-1表示最后一个元素</param>
//	/// <returns></returns>
//	public static bool LTrim(string key, long start, long stop) => Instance.LTrim(key, start, stop);
//	#endregion

//	#region Set 操作
//	/// <summary>
//	/// 向集合添加一个或多个成员
//	/// </summary>
//	/// <param name="key">不含prefix前辍</param>
//	/// <param name="members">一个或多个成员</param>
//	/// <returns></returns>
//	public static long SAdd(string key, params string[] members) => Instance.SAdd(key, members);
//	/// <summary>
//	/// 获取集合的成员数
//	/// </summary>
//	/// <param name="key">不含prefix前辍</param>
//	/// <returns></returns>
//	public static long SCard(string key) => Instance.SCard(key);
//	/// <summary>
//	/// 返回给定所有集合的差集，警告：群集模式下，若keys分散在多个节点时，将报错
//	/// </summary>
//	/// <param name="keys">不含prefix前辍</param>
//	/// <returns></returns>
//	public static string[] SDiff(params string[] keys) => Instance.SDiff(keys);
//	/// <summary>
//	/// 返回给定所有集合的差集并存储在 destination 中，警告：群集模式下，若keys分散在多个节点时，将报错
//	/// </summary>
//	/// <param name="destination">新的无序集合，不含prefix前辍</param>
//	/// <param name="keys">一个或多个无序集合，不含prefix前辍</param>
//	/// <returns></returns>
//	public static long SDiffStore(string destination, params string[] keys) => Instance.SDiffStore(destination, keys);
//	/// <summary>
//	/// 返回给定所有集合的交集，警告：群集模式下，若keys分散在多个节点时，将报错
//	/// </summary>
//	/// <param name="keys">不含prefix前辍</param>
//	/// <returns></returns>
//	public static string[] SInter(params string[] keys) => Instance.SInter(keys);
//	/// <summary>
//	/// 返回给定所有集合的交集并存储在 destination 中，警告：群集模式下，若keys分散在多个节点时，将报错
//	/// </summary>
//	/// <param name="destination">新的无序集合，不含prefix前辍</param>
//	/// <param name="keys">一个或多个无序集合，不含prefix前辍</param>
//	/// <returns></returns>
//	public static long SInterStore(string destination, params string[] keys) => Instance.SInterStore(destination, keys);
//	/// <summary>
//	/// 返回集合中的所有成员
//	/// </summary>
//	/// <param name="key">不含prefix前辍</param>
//	/// <returns></returns>
//	public static string[] SMembers(string key) => Instance.SMembers(key);
//	/// <summary>
//	/// 将 member 元素从 source 集合移动到 destination 集合
//	/// </summary>
//	/// <param name="source">无序集合key，不含prefix前辍</param>
//	/// <param name="destination">目标无序集合key，不含prefix前辍</param>
//	/// <param name="member">成员</param>
//	/// <returns></returns>
//	public static bool SMove(string source, string destination, string member) => Instance.SMove(source, destination, member);
//	/// <summary>
//	/// 移除并返回集合中的一个随机元素
//	/// </summary>
//	/// <param name="key">不含prefix前辍</param>
//	/// <returns></returns>
//	public static string SPop(string key) => Instance.SPop(key);
//	/// <summary>
//	/// 返回集合中一个或多个随机数的元素
//	/// </summary>
//	/// <param name="key">不含prefix前辍</param>
//	/// <param name="count">返回个数</param>
//	/// <returns></returns>
//	public static string[] SRandMember(string key, int count = 1) => Instance.SRandMember(key, count);
//	/// <summary>
//	/// 移除集合中一个或多个成员
//	/// </summary>
//	/// <param name="key">不含prefix前辍</param>
//	/// <param name="members">一个或多个成员</param>
//	/// <returns></returns>
//	public static long SRem(string key, params string[] members) => Instance.SRem(key, members);
//	/// <summary>
//	/// 返回所有给定集合的并集，警告：群集模式下，若keys分散在多个节点时，将报错
//	/// </summary>
//	/// <param name="keys">不含prefix前辍</param>
//	/// <returns></returns>
//	public static string[] SUnion(params string[] keys) => Instance.SUnion(keys);
//	/// <summary>
//	/// 所有给定集合的并集存储在 destination 集合中，警告：群集模式下，若keys分散在多个节点时，将报错
//	/// </summary>
//	/// <param name="destination">新的无序集合，不含prefix前辍</param>
//	/// <param name="keys">一个或多个无序集合，不含prefix前辍</param>
//	/// <returns></returns>
//	public static long SUnionStore(string destination, params string[] keys) => Instance.SUnionStore(destination, keys);
//	/// <summary>
//	/// 迭代集合中的元素
//	/// </summary>
//	/// <param name="key">不含prefix前辍</param>
//	/// <param name="cursor">位置</param>
//	/// <param name="pattern">模式</param>
//	/// <param name="count">数量</param>
//	/// <returns></returns>
//	public static RedisScan<string> SScan(string key, int cursor, string pattern = null, int? count = null) => Instance.SScan(key, cursor, pattern, count);
//	#endregion

//	#region Sorted Set 操作
//	/// <summary>
//	/// 向有序集合添加一个或多个成员，或者更新已存在成员的分数
//	/// </summary>
//	/// <param name="key">不含prefix前辍</param>
//	/// <param name="scoreMembers">一个或多个成员分数</param>
//	/// <returns></returns>
//	public static long ZAdd(string key, params (double, string)[] scoreMembers) => Instance.ZAdd(key, scoreMembers);
//	/// <summary>
//	/// 获取有序集合的成员数量
//	/// </summary>
//	/// <param name="key">不含prefix前辍</param>
//	/// <returns></returns>
//	public static long ZCard(string key) => Instance.ZCard(key);
//	/// <summary>
//	/// 计算在有序集合中指定区间分数的成员数量
//	/// </summary>
//	/// <param name="key">不含prefix前辍</param>
//	/// <param name="min">分数最小值</param>
//	/// <param name="max">分数最大值</param>
//	/// <returns></returns>
//	public static long ZCount(string key, double min, double max) => Instance.ZCount(key, min, max);
//	/// <summary>
//	/// 有序集合中对指定成员的分数加上增量 increment
//	/// </summary>
//	/// <param name="key">不含prefix前辍</param>
//	/// <param name="memeber">成员</param>
//	/// <param name="increment">增量值(默认=1)</param>
//	/// <returns></returns>
//	public static double ZIncrBy(string key, string memeber, double increment = 1) => Instance.ZIncrBy(key, memeber, increment);

//	#region 多个有序集合 交集
//	/// <summary>
//	/// 计算给定的一个或多个有序集的最大值交集，将结果集存储在新的有序集合 destination 中，警告：群集模式下，若keys分散在多个节点时，将报错
//	/// </summary>
//	/// <param name="destination">新的有序集合，不含prefix前辍</param>
//	/// <param name="keys">一个或多个有序集合，不含prefix前辍</param>
//	/// <returns></returns>
//	public static long ZInterStoreMax(string destination, params string[] keys) => Instance.ZInterStoreMax(destination, keys);
//	/// <summary>
//	/// 计算给定的一个或多个有序集的最小值交集，将结果集存储在新的有序集合 destination 中，警告：群集模式下，若keys分散在多个节点时，将报错
//	/// </summary>
//	/// <param name="destination">新的有序集合，不含prefix前辍</param>
//	/// <param name="keys">一个或多个有序集合，不含prefix前辍</param>
//	/// <returns></returns>
//	public static long ZInterStoreMin(string destination, params string[] keys) => Instance.ZInterStoreMin(destination, keys);
//	/// <summary>
//	/// 计算给定的一个或多个有序集的合值交集，将结果集存储在新的有序集合 destination 中，警告：群集模式下，若keys分散在多个节点时，将报错
//	/// </summary>
//	/// <param name="destination">新的有序集合，不含prefix前辍</param>
//	/// <param name="keys">一个或多个有序集合，不含prefix前辍</param>
//	/// <returns></returns>
//	public static long ZInterStoreSum(string destination, params string[] keys) => Instance.ZInterStoreSum(destination, keys);
//	#endregion

//	#region 多个有序集合 并集
//	/// <summary>
//	/// 计算给定的一个或多个有序集的最大值并集，将该并集(结果集)储存到 destination，警告：群集模式下，若keys分散在多个节点时，将报错
//	/// </summary>
//	/// <param name="destination">新的有序集合，不含prefix前辍</param>
//	/// <param name="keys">一个或多个有序集合，不含prefix前辍</param>
//	/// <returns></returns>
//	public static long ZUnionStoreMax(string destination, params string[] keys) => Instance.ZUnionStoreMax(destination, keys);
//	/// <summary>
//	/// 计算给定的一个或多个有序集的最小值并集，将该并集(结果集)储存到 destination，警告：群集模式下，若keys分散在多个节点时，将报错
//	/// </summary>
//	/// <param name="destination">新的有序集合，不含prefix前辍</param>
//	/// <param name="keys">一个或多个有序集合，不含prefix前辍</param>
//	/// <returns></returns>
//	public static long ZUnionStoreMin(string destination, params string[] keys) => Instance.ZUnionStoreMin(destination, keys);
//	/// <summary>
//	/// 计算给定的一个或多个有序集的合值并集，将该并集(结果集)储存到 destination，警告：群集模式下，若keys分散在多个节点时，将报错
//	/// </summary>
//	/// <param name="destination">新的有序集合，不含prefix前辍</param>
//	/// <param name="keys">一个或多个有序集合，不含prefix前辍</param>
//	/// <returns></returns>
//	public static long ZUnionStoreSum(string destination, params string[] keys) => Instance.ZUnionStoreSum(destination, keys);
//	#endregion

//	/// <summary>
//	/// 通过索引区间返回有序集合成指定区间内的成员
//	/// </summary>
//	/// <param name="key">不含prefix前辍</param>
//	/// <param name="start">开始位置，0表示第一个元素，-1表示最后一个元素</param>
//	/// <param name="stop">结束位置，0表示第一个元素，-1表示最后一个元素</param>
//	/// <returns></returns>
//	public static string[] ZRange(string key, long start, long stop) => Instance.ZRange(key, start, stop);
//	/// <summary>
//	/// 通过分数返回有序集合指定区间内的成员
//	/// </summary>
//	/// <param name="key">不含prefix前辍</param>
//	/// <param name="min">最小分数</param>
//	/// <param name="max">最大分数</param>
//	/// <param name="limit">返回多少成员</param>
//	/// <param name="offset">返回条件偏移位置</param>
//	/// <returns></returns>
//	public static string[] ZRangeByScore(string key, double min, double max, long? limit = null, long offset = 0) => Instance.ZRangeByScore(key, min, max, limit, offset);
//	/// <summary>
//	/// 通过分数返回有序集合指定区间内的成员和分数
//	/// </summary>
//	/// <param name="key">不含prefix前辍</param>
//	/// <param name="min">最小分数</param>
//	/// <param name="max">最大分数</param>
//	/// <param name="limit">返回多少成员</param>
//	/// <param name="offset">返回条件偏移位置</param>
//	/// <returns></returns>
//	public static (string member, double score)[] ZRangeByScoreWithScores(string key, double min, double max, long? limit = null, long offset = 0) => Instance.ZRangeByScoreWithScores(key, min, max, limit, offset);
//	/// <summary>
//	/// 返回有序集合中指定成员的索引
//	/// </summary>
//	/// <param name="key">不含prefix前辍</param>
//	/// <param name="member">成员</param>
//	/// <returns></returns>
//	public static long? ZRank(string key, string member) => Instance.ZRank(key, member);
//	/// <summary>
//	/// 移除有序集合中的一个或多个成员
//	/// </summary>
//	/// <param name="key">不含prefix前辍</param>
//	/// <param name="member">一个或多个成员</param>
//	/// <returns></returns>
//	public static long ZRem(string key, params string[] member) => Instance.ZRem(key, member);
//	/// <summary>
//	/// 移除有序集合中给定的排名区间的所有成员
//	/// </summary>
//	/// <param name="key">不含prefix前辍</param>
//	/// <param name="start">开始位置，0表示第一个元素，-1表示最后一个元素</param>
//	/// <param name="stop">结束位置，0表示第一个元素，-1表示最后一个元素</param>
//	/// <returns></returns>
//	public static long ZRemRangeByRank(string key, long start, long stop) => Instance.ZRemRangeByRank(key, start, stop);
//	/// <summary>
//	/// 移除有序集合中给定的分数区间的所有成员
//	/// </summary>
//	/// <param name="key">不含prefix前辍</param>
//	/// <param name="min">最小分数</param>
//	/// <param name="max">最大分数</param>
//	/// <returns></returns>
//	public static long ZRemRangeByScore(string key, double min, double max) => Instance.ZRemRangeByScore(key, min, max);
//	/// <summary>
//	/// 返回有序集中指定区间内的成员，通过索引，分数从高到底
//	/// </summary>
//	/// <param name="key">不含prefix前辍</param>
//	/// <param name="start">开始位置，0表示第一个元素，-1表示最后一个元素</param>
//	/// <param name="stop">结束位置，0表示第一个元素，-1表示最后一个元素</param>
//	/// <returns></returns>
//	public static string[] ZRevRange(string key, long start, long stop) => Instance.ZRevRange(key, start, stop);
//	/// <summary>
//	/// 返回有序集中指定分数区间内的成员，分数从高到低排序
//	/// </summary>
//	/// <param name="key">不含prefix前辍</param>
//	/// <param name="min">最小分数</param>
//	/// <param name="max">最大分数</param>
//	/// <param name="limit">返回多少成员</param>
//	/// <param name="offset">返回条件偏移位置</param>
//	/// <returns></returns>
//	public static string[] ZRevRangeByScore(string key, double max, double min, long? limit = null, long offset = 0) => Instance.ZRevRangeByScore(key, max, min, limit, offset);
//	/// <summary>
//	/// 返回有序集中指定分数区间内的成员和分数，分数从高到低排序
//	/// </summary>
//	/// <param name="key">不含prefix前辍</param>
//	/// <param name="min">最小分数</param>
//	/// <param name="max">最大分数</param>
//	/// <param name="limit">返回多少成员</param>
//	/// <param name="offset">返回条件偏移位置</param>
//	/// <returns></returns>
//	public static (string member, double score)[] ZRevRangeByScoreWithScores(string key, double max, double min, long? limit = null, long offset = 0) => Instance.ZRevRangeByScoreWithScores(key, max, min, limit, offset);
//	/// <summary>
//	/// 返回有序集合中指定成员的排名，有序集成员按分数值递减(从大到小)排序
//	/// </summary>
//	/// <param name="key">不含prefix前辍</param>
//	/// <param name="member">成员</param>
//	/// <returns></returns>
//	public static long? ZRevRank(string key, string member) => Instance.ZRevRank(key, member);
//	/// <summary>
//	/// 返回有序集中，成员的分数值
//	/// </summary>
//	/// <param name="key">不含prefix前辍</param>
//	/// <param name="member">成员</param>
//	/// <returns></returns>
//	public static double? ZScore(string key, string member) => Instance.ZScore(key, member);
//	#endregion

//}