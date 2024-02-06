using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CommandManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField m_text;

    [SerializeField] private List<string> m_log;

    private int m_logSize = 10;

    private void Start()
    {
        m_text.onEndEdit.AddListener(EndEdit);
        m_log = new(m_logSize);
    }

    private void EndEdit(string text_)
    {
        var _command = text_;
        
        if (m_log.Count == m_logSize)
        {
            m_log.RemoveAt(0);
        }
        m_log.Add(_command);

        var _name = _command[.._command.IndexOf(' ')];
        _command = _command[_name.Length..];
        var _argment = _command.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        var _processes = new Command[]
        {
            new Log(),
            new Spawn(),
            new DeSpawn(),
        };

        foreach (var process in _processes)
        {
            foreach (var name in process.CommandName)
            {
                if (name == _name)
                {
                    var _type = process.GetType();
                    var _methods = _type.GetMethods();
                    foreach (var method in _methods)
                    {
                        var _parametors = method.GetParameters();
                        foreach (var param in _parametors)
                        {
                            Debug.Log(param.Name);
                        }
                    }
                    process.Process(_argment);
                    goto finish;
                }
            }
        }

        finish:

        m_text.text = string.Empty;
    }

    public abstract class Command
    {
        public abstract string[] CommandName { get; protected set; }
        public abstract void Process(string[] argument_);
    }

    public class Log : Command
    {
        public override string[] CommandName { get; protected set; } = new string[]
        {
            "log",
        };
        public override void Process(string[] argument_)
        {
            Debug.Log(string.Join(',', argument_));
        }
    }
    public class Spawn : Command
    {
        public override string[] CommandName { get; protected set; } = new string[]
        {
            "spawn",
        };
        public override void Process(string[] argument_)
        {
            var _go = new GameObject();
            _go.name = argument_[0];
        }
    }
    public class DeSpawn : Command
    {
        public override string[] CommandName { get; protected set; } = new string[]
        {
            "despawn",
        };
        public override void Process(string[] argument_)
        {

        }
    }
}