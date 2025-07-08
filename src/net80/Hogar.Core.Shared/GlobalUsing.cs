global using System;
global using System.Net;
global using System.Linq;
global using System.Text;
global using System.Text.Json;
global using System.Reflection;
global using System.Collections;
global using System.Globalization;
global using System.Buffers.Binary;
global using System.ComponentModel;
global using System.Linq.Expressions;
global using System.Text.Encodings.Web;
global using System.Collections.Generic;
global using System.Security.Cryptography;
global using System.Collections.Concurrent;
global using System.Text.Json.Serialization;
global using System.Text.RegularExpressions;

global using FluentValidation;
global using FluentValidation.Results;
global using Microsoft.AspNetCore.Http;
global using Microsoft.Net.Http.Headers;
global using Microsoft.Extensions.Options;
global using Microsoft.AspNetCore.WebUtilities;
global using Microsoft.Extensions.DependencyInjection;

global using Hogar.Core.Shared.Utils;
global using Hogar.Core.Shared.Services;
global using Hogar.Core.Shared.Settings;
global using Hogar.Core.Shared.Extensions;
global using Hogar.Core.Shared.Exceptions;
global using Hogar.Core.Shared.Converters;

global using MainConstantsCore = Hogar.Core.Shared.Constants.MainConstants;
global using RegexConstantsCore = Hogar.Core.Shared.Constants.RegexConstants;
global using FormatConstantsCore = Hogar.Core.Shared.Constants.FormatConstants;
global using MessageConstantsCore = Hogar.Core.Shared.Constants.MessageConstants;
global using EnvironmentConstantsCore = Hogar.Core.Shared.Constants.EnvironmentConstants;
global using HttpStatusCodeConstantsCore = Hogar.Core.Shared.Constants.HttpStatusCodeConstants;
