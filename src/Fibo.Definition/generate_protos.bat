..\..\..\gRPC\tools\2.25.0\protoc.exe -I protos --csharp_out .\ --grpc_out .\ protos\fibo.proto --plugin=protoc-gen-grpc=..\..\..\gRPC\tools\2.25.0\grpc_csharp_plugin.exe

@rem grpcc --proto fibo.proto -i --address 127.0.0.1:50051

@rem client.getFiboNumberByIndex({index: 3}, pr);

@rem client.GetVisitedValues({}, pr);

@rem docker run -p 5432:5432 -e POSTGRES_PASSWORD=password01 -e POSTGRES_USER=superuser -e POSTGRES_DB=maindb -d postgres
@rem docker run -p 6379:6379 redis