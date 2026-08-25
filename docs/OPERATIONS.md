# Operations and Reliability

## Starting the Integration

A production operator or support user:

1. opens the application while connected to the authorized company network;
2. verifies that configuration and database initialization succeed;
3. starts synchronization;
4. confirms that the health indicator shows an active state;
5. monitors transfer activity when needed.

## Normal Operation

During normal operation the application:

- repeatedly checks for new source transactions;
- transfers only records not already present downstream;
- updates the total synchronized-record count;
- displays recent activity;
- writes persistent logs.

A period with no new transfers is not automatically an error. It may simply mean that no new warehouse scans are awaiting synchronization.

## Failure States

### Database connectivity failure
The application logs connectivity failures and surfaces them to the operator.

### Repeated lack of transfers
If receiving activity is expected but no downstream transfers occur, the integration path should be investigated.

### Application stopped
When synchronization is stopped, no new source records are transferred until the process is restarted.

## Recovery

Because the application uses incremental duplicate-aware synchronization, pending source records can be picked up after the application or connection is restored.

## Operational Evidence Maintained Privately

The production environment may retain:

- daily application logs;
- transfer ledgers;
- deployment records;
- support tickets;
- screenshots;
- transaction-volume statistics.

These materials are not included in the public repository because they may contain confidential employer or operational information.
