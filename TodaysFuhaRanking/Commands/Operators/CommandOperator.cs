﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using NLog;
using TodaysFuhaRanking.Settings;
using TodaysFuhaRanking.Settings.Repositories;

namespace TodaysFuhaRanking.Commands.Operators
{
    /// <summary>
    /// コマンド ライン引数によるコマンドの操作を行うクラスを表します。
    /// </summary>
    public class CommandOperator
    {
        /// <summary>コマンド ライン引数</summary>
        private readonly CommandLineArgs args;
        /// <summary>ロギング オブジェクト</summary>
        private readonly ILogger logger;
        /// <summary>アプリケーション設定</summary>
        private readonly SettingsRoot settings;

        /// <summary>
        /// <see cref="CommandOperator"/> の新しいインスタンスを生成します。
        /// </summary>
        /// <param name="args">コマンド ライン引数。</param>
        public CommandOperator(IEnumerable<string> args)
        {
            this.args = CommandLineArgs.ParseFrom(args);

            logger = LogManager.GetCurrentClassLogger();

            var repository = SettingsRepositoryFactory.CreateDefault();
            settings = repository.Load();
        }

        /// <summary>
        /// 実行可能なコマンドを全て実行します。
        /// </summary>
        public void ExecuteCommands()
        {
            var commands = CreateCommands();
            if (!commands.Any()) {
                throw new InvalidOperationException("実行する機能が一つも指定されていません。"); 
            }

            foreach (var command in commands)
            {
                command.Execute(null);
            }
        }

        /// <summary>
        /// コマンドのコレクションを生成します。
        /// </summary>
        /// <returns>実行オプションを元に生成されたコマンドのコレクション。</returns>
        private IEnumerable<ICommand> CreateCommands()
        {
            if (args.ExecutesAggregate) { yield return new AggregateCommand(logger, settings); }
            if (args.ExecutesTweet) { yield return new TweetCommand(logger, settings); }
            if (args.ExecutesExportText) { yield return new ExportTextCommand(logger, settings); }
        }
    }
}
