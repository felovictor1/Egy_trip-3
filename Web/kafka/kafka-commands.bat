@echo off
REM =======================================================
REM  Kafka Topic Management - Docker Edition
REM  Run this AFTER: docker-compose up -d
REM =======================================================

echo.
echo ====================================================
echo  STEP 1: Create Topics
echo ====================================================

echo [1/2] Creating 'order' topic with 6 partitions...
docker exec assignment-kafka1-1 kafka-topics --bootstrap-server kafka1:29092 --create --topic order --partitions 6 --replication-factor 1

echo [2/2] Creating 'payment' topic with 2 partitions...
docker exec assignment-kafka1-1 kafka-topics --bootstrap-server kafka1:29092 --create --topic payment --partitions 2 --replication-factor 1

echo.
echo ====================================================
echo  STEP 2: Verify Topics Were Created
echo ====================================================

echo Listing all topics...
docker exec assignment-kafka1-1 kafka-topics --bootstrap-server kafka1:29092 --list

echo Describing 'order' topic...
docker exec assignment-kafka1-1 kafka-topics --bootstrap-server kafka1:29092 --describe --topic order

echo Describing 'payment' topic...
docker exec assignment-kafka1-1 kafka-topics --bootstrap-server kafka1:29092 --describe --topic payment

echo.
echo ====================================================
echo  STEP 3: Apply Business Requirement Changes
echo ====================================================

echo [payment] Increasing partitions from 2 to 4...
docker exec assignment-kafka1-1 kafka-topics --bootstrap-server kafka1:29092 --alter --topic payment --partitions 4

echo [order] Deleting topic (cannot reduce partitions in Kafka)...
docker exec assignment-kafka1-1 kafka-topics --bootstrap-server kafka1:29092 --delete --topic order

echo Waiting 5 seconds for deletion to complete...
timeout /t 5 /nobreak >nul

echo [order] Recreating with 4 partitions...
docker exec assignment-kafka1-1 kafka-topics --bootstrap-server kafka1:29092 --create --topic order --partitions 4 --replication-factor 1

echo.
echo ====================================================
echo  STEP 4: Verify Final State
echo ====================================================

echo Describing 'order' topic (expected: 4 partitions)...
docker exec assignment-kafka1-1 kafka-topics --bootstrap-server kafka1:29092 --describe --topic order

echo Describing 'payment' topic (expected: 4 partitions)...
docker exec assignment-kafka1-1 kafka-topics --bootstrap-server kafka1:29092 --describe --topic payment

echo.
echo ====================================================
echo  DONE! Both topics now have 4 partitions each.
echo ====================================================
pause
