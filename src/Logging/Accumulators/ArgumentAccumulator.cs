using System;
using System.Collections.Generic;
using System.Text;

namespace VentLib.Logging.Accumulators;

internal class ArgumentAccumulator : ILogAccumulator
{
    public string AccumulatorID => nameof(ArgumentAccumulator);

    public LogComposite Accumulate(LogComposite composite, LogArguments arguments)
    {
        string initialString = composite.Message;

        StringBuilder builder = new();
        builder.EnsureCapacity(initialString.Length + arguments.Arguments.Length * 8);

        Stack<string> stack = new();
        int pos = 0;

        while (pos < initialString.Length)
        {
            char c = initialString[pos++];

            switch (c)
            {
                case '{':
                    if (pos < initialString.Length && initialString[pos] == '{')
                    {
                        // Escaped opening brace, treat as literal
                        builder.Append('{');
                        pos++; // skip next '{'
                    }
                    else
                    {
                        // Start of a placeholder
                        stack.Push("{");
                    }
                    break;

                case '}':
                    if (stack.Count > 0 && stack.Peek() == "{")
                    {
                        // End of a placeholder
                        string placeholder = stack.Pop();
                        if (!int.TryParse(placeholder, out int argNum) || argNum >= arguments.Arguments.Length)
                        {
                            // Invalid placeholder, treat as literal
                            builder.Append($"{{Invalid Placeholder: {placeholder}}}");
                        }
                        else
                        {
                            // Valid placeholder
                            builder.Append(arguments.Arguments[argNum]);
                        }
                    }
                    else if (pos < initialString.Length && initialString[pos] == '}')
                    {
                        // Escaped closing brace, treat as literal
                        builder.Append('}');
                        pos++; // skip next '}'
                    }
                    else
                    {
                        // Unmatched brace, treat as literal
                        builder.Append(c);
                    }
                    break;

                case '"':
                    // Escape quotation marks to maintain JSON format integrity
                    builder.Append("\\\"");
                    break;

                case '\\':
                    // Handle escape sequences for quotes or backslashes
                    if (pos < initialString.Length && initialString[pos] == '"')
                    {
                        builder.Append("\\\"");
                        pos++; // skip the next quote
                    }
                    else
                    {
                        builder.Append('\\');
                    }
                    break;

                default:
                    if (stack.Count > 0 && stack.Peek() == "{")
                    {
                        // Inside placeholder
                        stack.Push(stack.Pop() + c);
                    }
                    else
                    {
                        // Regular character, just append it
                        builder.Append(c);
                    }
                    break;
            }
        }

        // If we end with an open placeholder, treat it as an invalid placeholder
        if (stack.Count > 0 && stack.Peek() == "{")
        {
            builder.Append($"{{Invalid Placeholder: {stack.Pop()}}}");
        }

        return composite.SetMessage(builder.ToString());
    }
}
