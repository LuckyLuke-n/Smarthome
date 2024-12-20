#!/bin/bash

# Wait for Redis to start
sleep 5

# Create a user with a password and specific permissions
redis-cli -h localhost -p 6379 -a admin <<EOF
ACL SETUSER api on >api-pw ~* +@all
EOF