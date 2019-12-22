..\..\..\gRPC\tools\2.25.0\protoc.exe -I protos --csharp_out .\ --grpc_out .\ protos\fibo.proto --plugin=protoc-gen-grpc=..\..\..\gRPC\tools\2.25.0\grpc_csharp_plugin.exe

@rem grpcc --proto fibo.proto -i --address 127.0.0.1:50051

@rem client.getFiboNumberByIndex({index: 3}, pr);