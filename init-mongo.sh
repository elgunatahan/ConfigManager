echo "Waiting for MongoDB to start..."
sleep 10

echo "Initiating replica set..."
mongosh --eval 'rs.initiate()'