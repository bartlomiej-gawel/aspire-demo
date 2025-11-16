<script setup lang="ts">
import { computed } from 'vue'
import { mockOrganizations } from '@/data/mock-organizations'
import {
  getEmployeeStatusLabel,
  getEmployeeStatusVariant,
  formatDate,
} from '@/lib/organization-utils'
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '@/components/ui/table'
import { Badge } from '@/components/ui/badge'
import { Button } from '@/components/ui/button'
import { ArrowLeft } from 'lucide-vue-next'

interface Props {
  organizationId: string
}

const props = defineProps<Props>()

const organization = computed(() => {
  return mockOrganizations.find(org => org.id === props.organizationId)
})

const employees = computed(() => {
  return organization.value?.employees || []
})
</script>

<template>
  <div class="flex flex-1 flex-col gap-4">
    <div class="flex items-center justify-between">
      <div>
        <Button variant="ghost" size="sm" as-child class="mb-2">
          <a href="#organizations">
            <ArrowLeft class="mr-2 h-4 w-4" />
            Back to Organizations
          </a>
        </Button>
        <h2 class="text-2xl font-bold tracking-tight">
          Employees - {{ organization?.name || 'Unknown' }}
        </h2>
        <p class="text-muted-foreground">
          View all employees for this organization
        </p>
      </div>
    </div>

    <div class="rounded-md border">
      <Table>
        <TableHeader>
          <TableRow>
            <TableHead>Name</TableHead>
            <TableHead>Email</TableHead>
            <TableHead>Phone</TableHead>
            <TableHead>Role</TableHead>
            <TableHead>Status</TableHead>
            <TableHead>Created At</TableHead>
          </TableRow>
        </TableHeader>
        <TableBody>
          <TableRow v-if="employees.length === 0">
            <TableCell colspan="6" class="text-center text-muted-foreground">
              No employees found
            </TableCell>
          </TableRow>
          <TableRow v-for="employee in employees" :key="employee.id">
            <TableCell class="font-medium py-3">
              {{ employee.firstName }} {{ employee.lastName }}
            </TableCell>
            <TableCell class="py-3">
              {{ employee.email }}
            </TableCell>
            <TableCell class="py-3">
              {{ employee.phone || 'N/A' }}
            </TableCell>
            <TableCell class="py-3">
              <Badge variant="outline">
                {{ employee.role === 1 ? 'Admin' : employee.role === 2 ? 'Manager' : 'Employee' }}
              </Badge>
            </TableCell>
            <TableCell class="py-3">
              <Badge :variant="getEmployeeStatusVariant(employee.status)">
                {{ getEmployeeStatusLabel(employee.status) }}
              </Badge>
            </TableCell>
            <TableCell class="text-muted-foreground py-3">
              {{ formatDate(employee.createdAt) }}
            </TableCell>
          </TableRow>
        </TableBody>
      </Table>
    </div>
  </div>
</template>
