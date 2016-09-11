- Logging is simplified to the form of "LogTo" which really calls Catel's ILog instance
  See https://github.com/Fody/Anotar
- Async methods exceptions automatically transit through AsyncErrorHandler class (Sidekick.Shared.AsyncErrorHandler)
  See https://github.com/Fody/AsyncErrorHandler
- Assembly name is weaved with Git informations (currently for Windows platform)
  See https://github.com/Fody/Stamp
