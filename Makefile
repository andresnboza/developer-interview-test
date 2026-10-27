.PHONY: dev test

# make dev                        -> prompts for inputs
# make dev ARGS="RATE WIDGET 100" -> uses command line arguments
dev:
	dotnet run --project Smartwyre.DeveloperTest.Runner -- $(ARGS)

test:
	dotnet test Smartwyre.DeveloperTest.Tests

